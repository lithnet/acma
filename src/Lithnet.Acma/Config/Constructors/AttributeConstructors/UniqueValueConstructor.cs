// -----------------------------------------------------------------------
// <copyright file="UniqueValueConstructor.cs" company="Lithnet">
// Copyright (c) 2013
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
    using Microsoft.MetadirectoryServices;
    using Microsoft.Win32;

    /// <summary>
    /// A constructor used to create a unique attribute value using a declarative syntax
    /// </summary>
    [DataContract(Name = "unique-value-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Unique value")]
    public class UniqueValueConstructor : AttributeConstructor
    {
        /// <summary>
        /// Initializes a new instance of the UniqueValueConstructor class
        /// </summary>
        public UniqueValueConstructor()
            : base()
        {
            this.Initialize();
            this.GetAlgorithmMode();
        }

        public static bool DisableCaching { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes that must be tested against for uniqueness before assigning a value to this attribute
        /// </summary>
        [DataMember(Name = "unique-allocation-attributes")]
        public List<AcmaSchemaAttribute> UniqueAllocationAttributes { get; set; }

        /// <summary>
        /// Gets or sets the value declaration that guarantees uniqueness for the specified attribute
        /// </summary>
        [DataMember(Name = "value-declaration")]
        public UniqueValueDeclaration ValueDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the list of static declarations to try before the unique declaration expanded
        /// </summary>
        [DataMember(Name = "static-declarations")]
        public List<ValueDeclaration> StaticDeclarations { get; set; }

        /// <summary>
        /// Gets or sets the value declaration string for this constructor
        /// </summary>
        public string ValueDeclarationString
        {
            get
            {
                return this.ValueDeclaration.Declaration;
            }

            set
            {
                this.ValueDeclaration.Declaration = value;
            }
        }

        private int AlgorithmMode { get; set; }

        /// <summary>
        /// Constructs a target attribute value based on the rules in the constructor
        /// </summary>
        /// <param name="hologram">The object to construct the value for</param>
        internal override void Execute(MAObjectHologram hologram)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            IList<object> returnValues = new List<object>();

            if (this.UniqueAllocationAttributes.Count == 0)
            {
                throw new NotSupportedException("There were no attributes to process unique allocations against");
            }

            string staticValue = this.TryGetUniqueValueFromStaticDeclarations(hologram);

            if (string.IsNullOrWhiteSpace(staticValue))
            {
                returnValues = this.ValueDeclaration.Expand(hologram, (string valueToTest, string wildCardValue) => this.IsAttributeUnique(hologram, valueToTest, wildCardValue));
            }
            else
            {
                returnValues.Add(staticValue);
            }

            if (returnValues == null || returnValues.Count == 0)
            {
                hologram.DeleteAttribute(this.Attribute);
            }
            else
            {
                if (returnValues.Count > 1)
                {
                    throw new TooManyValuesException(string.Format("The value declaration '{0}' returned more than one value. Only single valued attributes are supported for unique allocation", this.ValueDeclaration));
                }

                object newValue = TypeConverter.ConvertData(returnValues.First(), this.Attribute.Type);

                if (newValue != null && !(newValue is string && string.IsNullOrWhiteSpace((string)newValue)))
                {
                    valueChanges.Add(this.CreateValueChange(newValue, ValueModificationType.Add));
                }
            }

            this.ApplyValueChanges(hologram, valueChanges, AcmaAttributeModificationType.Replace);
            this.RaiseCompletedEvent();
        }

        protected override IEnumerable<SchemaAttributeUsage> GetAttributeUsageInternal(string parentPath, AcmaSchemaAttribute attribute)
        {
            SchemaAttributeUsage usage;

            foreach (ValueDeclaration value in this.StaticDeclarations)
            {
                usage = value.GetAttributeUsage(parentPath, attribute);

                if (usage != null)
                {
                    yield return usage;
                }
            }

            usage = this.ValueDeclaration.GetAttributeUsage(parentPath, attribute);

            if (usage != null)
            {
                yield return usage;
            }

            if (this.UniqueAllocationAttributes.Any(t => t == attribute))
            {
                yield return new SchemaAttributeUsage(this, this.GetType().Name, this.ID, parentPath, "Unique allocation attribute");
            }

        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Attribute")
            {
                if (this.Attribute == null)
                {
                    this.AddError("Attribute", "An attribute must be specified");
                }
                else
                {
                    if (this.Attribute.IsMultivalued)
                    {
                        this.AddError("Attribute", "Only single-valued attributes can be used on a unique allocation constructor");
                    }
                    else
                    {
                        if (this.Attribute.Type != ExtendedAttributeType.String)
                        {
                            this.AddError("Attribute", "Only attributes with a string data type can be used on a unique allocation constructor");
                        }
                        else
                        {
                            this.RemoveError("Attribute");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Expands the static declarations, and if any result in a value that is currently unique, that value is required
        /// </summary>
        /// <param name="hologram">The source object</param>
        /// <returns>A unique value expanded from the static declarations, or null if a unique value could not be found</returns>
        private string TryGetUniqueValueFromStaticDeclarations(MAObjectHologram hologram)
        {
            if (this.StaticDeclarations.Count == 0)
            {
                return null;
            }

            foreach (var staticDeclaration in this.StaticDeclarations)
            {
                IList<object> values = staticDeclaration.Expand(hologram);

                if (values.Count == 0)
                {
                    continue;
                }
                else if (values.Count > 1)
                {
                    throw new TooManyValuesException(string.Format("The value declaration '{0}' returned more than one value. Only single valued attributes are supported for unique allocation", staticDeclaration));
                }

                string value = TypeConverter.ConvertData<string>(values.First());

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                if (this.IsAttributeUnique(hologram, value))
                {
                    return value;
                }
            }

            return null;
        }

        /// <summary>
        /// Occurs after the object has been deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (this.UniqueAllocationAttributes.Count() == 0)
            {
                this.AddError("UniqueAllocationAttributes", "At least one unique allocation attribute is required");
            }
            else
            {
                this.RemoveError("UniqueAllocationAttributes");
            }

            this.GetAlgorithmMode();
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
            this.StaticDeclarations = new List<ValueDeclaration>();
            this.UniqueAllocationAttributes = new List<AcmaSchemaAttribute>();
            this.valueCache = new Dictionary<string, HashSet<string>>();
        }

        private void GetAlgorithmMode()
        {
            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey key = baseKey.OpenSubKey(@"Software\Lithnet\ACMA\UVC\Algorithm");

            if (key != null)
            {
                object value = key.GetValue(this.ID, null);
                if (value != null)
                {
                    this.AlgorithmMode = Int32.Parse(value.ToString());
                }
                else
                {
                    value = key.GetValue(null, null);

                    if (value != null)
                    {
                        this.AlgorithmMode = Int32.Parse(value.ToString());
                    }
                }

            }
        }

        private Dictionary<string, HashSet<string>> valueCache;

        private void PopulateValueCache(MAObjectHologram hologram, string wilcardValue)
        {
            if (!this.valueCache.ContainsKey(wilcardValue))
            {
                this.valueCache.Add(wilcardValue, new HashSet<string>(StringComparer.CurrentCultureIgnoreCase));

                foreach (AcmaSchemaAttribute attribute in this.UniqueAllocationAttributes)
                {
                    foreach (string item in ActiveConfig.DB.GetAttributeValues(attribute, wilcardValue, hologram.ObjectID))
                    {
                        this.valueCache[wilcardValue].Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a particular attribute value is unique
        /// </summary>
        /// <param name="hologram">The object that the value is being tested for</param>
        /// <param name="valueToTest">The value test for uniqueness</param>
        /// <returns>True, if the value is unique, false if it is not</returns>
        internal bool IsAttributeUnique(MAObjectHologram hologram, string valueToTest, string wildcardValue)
        {
            if (this.AlgorithmMode == 0)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(wildcardValue))
                    {
                        throw new ArgumentNullException("wildcardValue");
                    }

                    if (string.IsNullOrWhiteSpace(valueToTest))
                    {
                        throw new ArgumentNullException("valueToTest");
                    }

                    this.PopulateValueCache(hologram, wildcardValue);

                    if (this.valueCache[wildcardValue].Add(valueToTest))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    if (UniqueValueConstructor.DisableCaching)
                    {
                        this.valueCache = new Dictionary<string, HashSet<string>>();
                    }

                }
            }
            else
            {
                return this.IsAttributeUnique(hologram, valueToTest);
            }
        }

        /// <summary>
        /// Determines if a particular attribute value is unique
        /// </summary>
        /// <param name="hologram">The object that the value is being tested for</param>
        /// <param name="valueToTest">The value test for uniqueness</param>
        /// <returns>True, if the value is unique, false if it is not</returns>
        internal bool IsAttributeUnique(MAObjectHologram hologram, string valueToTest)
        {
            foreach (AcmaSchemaAttribute attribute in this.UniqueAllocationAttributes)
            {
                if (ActiveConfig.DB.DoesAttributeValueExist(attribute, valueToTest, hologram.ObjectID))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
