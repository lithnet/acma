namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Performs a lookup against an internal table within the class and returns a replacement value
    /// </summary>
    [DataContract(Name = "simple-lookup", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Simple lookup")]
    public class SimpleLookupTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the SimpleLookupTransform class
        /// </summary>
        public SimpleLookupTransform()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the data type to return the matched value as
        /// </summary>
        [DataMember(Name = "return-type")]
        public ExtendedAttributeType UserDefinedReturnType { get; set; }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Gets or sets the list of lookup items
        /// </summary>
        [DataMember(Name = "lookup-items")]
        public ObservableCollection<LookupItem> LookupItems { get; set; }

        /// <summary>
        /// Gets or sets the action to take when no match was found in the table
        /// </summary>
        [DataMember(Name = "on-missing-match")]
        public OnMissingMatch OnMissingMatch { get; set; }

        /// <summary>
        /// Gets or sets the default value to apply if no match was found in the table
        /// </summary>
        [DataMember(Name = "default-value")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Performs the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            object returnValue = this.LookupReplacement(TypeConverter.ConvertData<string>(inputValue));
            return TypeConverter.ConvertData(returnValue, this.UserDefinedReturnType);
        }

        /// <summary>
        /// Gets the replacement value from the internal lookup table
        /// </summary>
        /// <param name="inputValue">The value to lookup</param>
        /// <returns>The replacement value</returns>
        private object LookupReplacement(string inputValue)
        {
            LookupItem match = this.LookupItems.FirstOrDefault(
                t => !string.IsNullOrWhiteSpace(t.CurrentValue) && t.CurrentValue.Equals(inputValue, StringComparison.CurrentCultureIgnoreCase));

            if (match == null)
            {
                switch (this.OnMissingMatch)
                {
                    case OnMissingMatch.UseNull:
                        return null;

                    case OnMissingMatch.UseOriginal:
                        return inputValue;

                    case OnMissingMatch.UseDefault:
                        return this.DefaultValue;

                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }
            else
            {
                return match.NewValue;
            }
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.LookupItems = new ObservableCollection<LookupItem>();
            this.UserDefinedReturnType = ExtendedAttributeType.String;
        }

        /// <summary>
        /// Performs pre-deserialization initialization
        /// </summary>
        /// <param name="context">The current serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
