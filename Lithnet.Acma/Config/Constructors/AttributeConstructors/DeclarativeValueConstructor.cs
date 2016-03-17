// -----------------------------------------------------------------------
// <copyright file="DeclarativeValueConstructor.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A constructor used to create an attribute value using a declarative syntax
    /// </summary>
    [DataContract(Name = "declarative-value-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Value declaration")]
    public class DeclarativeValueConstructor : AttributeConstructor
    {
        private ValueChangeComparer valueChangeComparer;

        /// <summary>
        /// The conditional presence rules that apply to this object
        /// </summary>
        private RuleGroup presenceConditions;

        /// <summary>
        /// Initializes a new instance of the DeclarativeValueConstructor class
        /// </summary>
        public DeclarativeValueConstructor()
            : base()
        {
            this.Initialize();
        }

        private ValueChangeComparer ValueChangeComparer
        {
            get
            {
                if (this.valueChangeComparer == null)
                {
                    this.valueChangeComparer = new ValueChangeComparer(this.Attribute, true);
                }

                return this.valueChangeComparer;
            }
        }

        /// <summary>
        /// Gets or sets the modification type to apply to value changes made by this constructor
        /// </summary>
        [DataMember(Name = "modification-type")]
        public AcmaAttributeModificationType ModificationType { get; set; }

        /// <summary>
        /// Gets or sets the value declarations for this constructor
        /// </summary>
        [DataMember(Name = "value-declarations")]
        public List<ValueDeclaration> ValueDeclarations { get; set; }

        /// <summary>
        /// Gets or sets the presence conditions for this constructor
        /// </summary>
        [DataMember(Name = "presence-conditions")]
        public RuleGroup PresenceConditions
        {
            get
            {
                return this.presenceConditions;
            }

            set
            {
                if (this.presenceConditions != null)
                {
                    this.presenceConditions.ObjectClassScopeProvider = null;
                }

                if (value != null)
                {
                    value.ObjectClassScopeProvider = this;
                }

                this.presenceConditions = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this constructor has presence conditions
        /// </summary>
        private bool HasPresenceConditions
        {
            get
            {
                return this.PresenceConditions != null && this.PresenceConditions.Items.Count > 0;
            }
        }

        /// <summary>
        /// Constructs a target attribute value based on the rules in the constructor
        /// </summary>
        /// <param name="hologram">The object to construct the value for</param>
        internal override void Execute(MAObjectHologram hologram)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();
            AcmaAttributeModificationType modificationType;

            if (this.ModificationType == AcmaAttributeModificationType.Conditional)
            {
                if (!this.HasPresenceConditions)
                {
                    throw new InvalidOperationException("The constructor has a conditional modification type, but no presence conditions were present");
                }

                if (this.PresenceConditions.Evaluate(hologram))
                {
                    if (this.Attribute.IsMultivalued)
                    {
                        modificationType = AcmaAttributeModificationType.Add;
                    }
                    else
                    {
                        modificationType = AcmaAttributeModificationType.Replace;
                    }

                    Logger.WriteLine("Presence conditions met", LogLevel.Debug);
                }
                else
                {
                    modificationType = AcmaAttributeModificationType.Delete;
                    Logger.WriteLine("Presence conditions not met", LogLevel.Debug);
                }
            }
            else
            {
                modificationType = this.ModificationType;
            }

            foreach (ValueDeclaration declaration in this.ValueDeclarations)
            {
                IList<object> returnValue = declaration.Expand(hologram);
               
                if (returnValue == null)
                {
                    continue;
                }
                else
                {
                    foreach (object value in returnValue)
                    {
                        object newValue = TypeConverter.ConvertData(value, this.Attribute.Type);
                        if (newValue != null && !(newValue is string && string.IsNullOrWhiteSpace((string)newValue)))
                        {
                            if (modificationType == AcmaAttributeModificationType.Delete)
                            {
                                if (hologram.HasAttributeValue(this.Attribute, newValue))
                                {
                                    valueChanges.Add(this.CreateValueChange(newValue, ValueModificationType.Delete));
                                }
                            }
                            else if (modificationType == AcmaAttributeModificationType.Add)
                            {
                                if (this.Attribute.IsMultivalued)
                                {
                                    if (!hologram.HasAttributeValue(this.Attribute, newValue))
                                    {
                                        valueChanges.Add(this.CreateValueChange(newValue, ValueModificationType.Add));
                                    }
                                }
                                else
                                {
                                    valueChanges.Add(this.CreateValueChange(newValue, ValueModificationType.Add));
                                }
                            }
                            else if (modificationType == AcmaAttributeModificationType.Replace)
                            {
                                valueChanges.Add(this.CreateValueChange(newValue, ValueModificationType.Add));
                            }
                            else
                            {
                                throw new InvalidOperationException("The modification type was unexpected at this location");
                            }
                        }
                    }
                }
            }

            valueChanges = valueChanges.Distinct(this.ValueChangeComparer).ToList();

            this.ApplyValueChanges(hologram, valueChanges, modificationType);
            this.RaiseCompletedEvent();
        }

        protected override IEnumerable<SchemaAttributeUsage> GetAttributeUsageInternal(string parentPath, AcmaSchemaAttribute attribute)
        {
            foreach (ValueDeclaration value in this.ValueDeclarations)
            {
                SchemaAttributeUsage usage = value.GetAttributeUsage(parentPath, attribute);

                if (usage != null)
                {
                    yield return usage;
                }
            }
        }

        /// <summary>
        /// Occurs after the object has been deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (this.ValueDeclarations == null)
            {
                this.ValueDeclarations = new List<ValueDeclaration>();
            }

            if (this.Attribute == null)
            {
                this.AddError("Attribute", "An attribute must be specified");
            }
            else
            {
                this.RemoveError("Attribute");

                if (!this.Attribute.IsMultivalued && this.ValueDeclarations.Count > 1)
                {
                    this.AddError("ValueDeclarations", "A single-valued attribute cannot contain multiple value-declaration elements");
                }
                else
                {
                    this.RemoveError("ValueDeclarations");
                }
            }

            if (this.ValueDeclarations.Any(t => t.InternalType == typeof(UniqueValueDeclaration)))
            {
                this.AddError("ValueDeclarations", "Unique allocation variables cannot be used in this constructor type");
            }
            else
            {
                this.RemoveError("ValueDeclarations");
            }
        }

        /// <summary>
        /// Occurs just prior to the object being deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.ValueDeclarations = new List<ValueDeclaration>();
            this.ModificationType = AcmaAttributeModificationType.Replace;
            this.PresenceConditions = new RuleGroup();
        }

        /// <summary>
        /// Converts an Acma attribute modification type to a MetadirectoryServices modification type
        /// </summary>
        /// <param name="modificationType">The Acma modification type</param>
        /// <returns>The MetadirectoryServices modification type</returns>
        private AttributeModificationType GetStandardModificationType(AcmaAttributeModificationType modificationType)
        {
            switch (modificationType)
            {
                case AcmaAttributeModificationType.Add:
                    return AttributeModificationType.Add;

                case AcmaAttributeModificationType.Replace:
                    return AttributeModificationType.Replace;

                case AcmaAttributeModificationType.Delete:
                    return AttributeModificationType.Delete;

                case AcmaAttributeModificationType.Conditional:
                    throw new InvalidOperationException("Cannot convert a conditional modification type to a MetadirectoryServices modification type");

                default:
                    throw new UnknownOrUnsupportedModificationTypeException();
            }
        }
    }
}
