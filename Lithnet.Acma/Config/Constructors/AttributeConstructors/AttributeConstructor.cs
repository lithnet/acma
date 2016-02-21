// ----------------------------------------------U-------------------------
// <copyright file="AttributeConstructor.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Defines a set of rules and parameters for constructing a target attribute value
    /// </summary>
    [KnownType(typeof(AttributeValueDeleteConstructor))]
    [KnownType(typeof(DeclarativeValueConstructor))]
    [KnownType(typeof(UniqueValueConstructor))]
    [KnownType(typeof(ReferenceLookupConstructor))]
    [KnownType(typeof(SequentialIntegerAllocationConstructor))]
    [DataContract(Name = "attribute-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public abstract class AttributeConstructor : ExecutableConstructorObject, IObjectClassScopeProvider, IAttributeScopeProvider
    {
        /// <summary>
        /// Initializes a new instance of the AttributeConstructor class
        /// </summary>
        public AttributeConstructor()
        {
            this.Initialize();
            this.ValidatePropertyChange("Id");
            this.ValidatePropertyChange("Attribute");
        }

        /// <summary>
        /// Represents a method that will handle the constructor completion event
        /// </summary>
        /// <param name="sender">The constructor that completed execution</param>
        public delegate void AttributeConstructorCompletedEventHandler(AttributeConstructor sender);

        /// <summary>
        /// Occurs when a constructor has completed execution
        /// </summary>
        public static event AttributeConstructorCompletedEventHandler AttributeConstructorCompletedEvent;

        /// <summary>
        /// Occurs when the execution of this constructor execution has completed
        /// </summary>
        public event AttributeConstructorCompletedEventHandler AttributeConstructorInstanceCompletedEvent;

        /// <summary>
        /// Gets or sets the attribute name that this constructor applies to
        /// </summary>
        [DataMember(Name = "attribute")]
        public AcmaSchemaAttribute Attribute { get; set; }

        /// <summary>
        /// Gets the name of this object
        /// </summary>
        public override string Name
        {
            get
            {
                if (this.Attribute == null)
                {
                    return this.ID;
                }
                else
                {
                    return string.Format("{0} ({1})", this.ID, this.Attribute.Name);
                }
            }
        }

        /// <summary>
        /// Evaluates the conditions of this constructor, and executes it if the rules are met
        /// </summary>
        /// <param name="hologram">The source object</param>
        /// <returns>A value indicating whether the constructor executed or not</returns>
        public override bool EvaluateAndExecute(MAObjectHologram hologram, IList<string> constructorOverrides)
        {
            try
            {
                if (this.HasErrors)
                {
                    throw new InvalidOperationException(string.Format("The constructor for attribute '{0}' with ID '{1}' has the following errors that must be fixed:\n{2}", this.Attribute.Name, this.ID, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
                }

                if (this.Disabled)
                {
                    Logger.WriteLine("Skipping {0} (disabled)", LogLevel.Debug, this.ID);
                    return false;
                }

                if (this.RuleGroup != null)
                {
                    if (!this.RuleGroup.Evaluate(hologram))
                    {
                        if (constructorOverrides != null && constructorOverrides.Contains(this.ID))
                        {
                            Logger.WriteLine("Execution rules overridden for {0}", LogLevel.Debug, this.ID);
                            this.Execute(hologram);
                            return true;
                        }
                        else
                        {
                            Logger.WriteLine("Skipping {0} (conditions not met)", LogLevel.Debug, this.ID);
                            return false;
                        }
                    }
                }

                try
                {
                    Logger.WriteLine("Executing {0}", this.ID);
                    Logger.IncreaseIndent();
                    this.Execute(hologram);
                }
                finally
                {
                    Logger.DecreaseIndent();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ConstructorExecutionException(this.ID, hologram.Id, ex);
            }
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            foreach (SchemaAttributeUsage usage in base.GetAttributeUsage(parentPath, attribute))
            {
                yield return usage;
            }

            string path = string.Format("{0}\\{1}", parentPath, this.ID);

            if (this.Attribute == attribute)
            {
                yield return new SchemaAttributeUsage(this, this.GetType().Name, this.ID, path, "Target attribute");
            }

            foreach (SchemaAttributeUsage usage in this.GetAttributeUsageInternal(path, attribute))
            {
                yield return usage;
            }
        }

        protected abstract IEnumerable<SchemaAttributeUsage> GetAttributeUsageInternal(string parentPath, AcmaSchemaAttribute attribute);
        
        /// <summary>
        /// Constructs a target attribute value based on the rules in the constructor
        /// </summary>
        /// <param name="hologram">The object to construct the value for</param>
        internal abstract void Execute(MAObjectHologram hologram);

        /// <summary>
        /// Raises the constructor completed event
        /// </summary>
        protected void RaiseCompletedEvent()
        {
            if (this.AttributeConstructorInstanceCompletedEvent != null)
            {
                this.AttributeConstructorInstanceCompletedEvent(this);
            }

            if (AttributeConstructorCompletedEvent != null)
            {
                AttributeConstructorCompletedEvent(this);
            }
        }

        /// <summary>
        /// Applies the value changes created by the constructor to the underlying object, using the requested modification type to define how to apply the new values
        /// </summary>
        /// <param name="hologram">The object to apply the changes to</param>
        /// <param name="valueChanges">The value changes to apply</param>
        /// <param name="modificationType">The type of modification to apply to the object</param>
        protected void ApplyValueChanges(MAObjectHologram hologram, IList<ValueChange> valueChanges, AcmaAttributeModificationType modificationType)
        {
            switch (modificationType)
            {
                case AcmaAttributeModificationType.Add:
                    hologram.UpdateAttributeValue(this.Attribute, valueChanges);
                    break;

                case AcmaAttributeModificationType.Replace:
                    hologram.SetAttributeValue(this.Attribute, valueChanges.Select(t => t.Value).ToList<object>());
                    break;

                case AcmaAttributeModificationType.Delete:
                    hologram.UpdateAttributeValue(this.Attribute, valueChanges);
                    break;

                default:
                    throw new InvalidOperationException("The specified modification type is unsupported for this constructor");
            }
        }

        /// <summary>
        /// Create a ValueChange for the specified value based on the modification type specified by this object
        /// </summary>
        /// <param name="newValue">The value to create the value change for</param>
        /// <param name="modificationType">The type of modification to perform</param>
        /// <returns>A new ValueChange object</returns>
        protected ValueChange CreateValueChange(object newValue, ValueModificationType modificationType)
        {
            if (modificationType == ValueModificationType.Delete)
            {
                return ValueChange.CreateValueDelete(newValue);
            }
            else
            {
                return ValueChange.CreateValueAdd(newValue);
            }
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "Attribute":

                    if (this.Attribute == null)
                    {
                        this.AddError("Attribute", "An attribute must be specified");
                    }
                    else
                    {
                        this.RemoveError("Attribute");
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
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
    }
}
