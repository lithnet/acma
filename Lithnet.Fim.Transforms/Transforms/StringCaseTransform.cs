namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Transforms the casing of a string
    /// </summary>
    [DataContract(Name = "string-case", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("String case")]
    public class StringCaseTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the StringCaseTransform class
        /// </summary>
        public StringCaseTransform()
        {
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
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
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.Reference;
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Gets or sets the casing to apply to the string
        /// </summary>
        [DataMember(Name = "string-case")]
        public StringCaseType StringCase { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.ModifyCase(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Changes the case of the input string
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>A copy of the original value with its case modified</returns>
        private object ModifyCase(string value)
        {
            switch (this.StringCase)
            {
                case StringCaseType.Lower:
                    return value.ToLower(CultureInfo.CurrentCulture);

                case StringCaseType.Upper:
                    return value.ToUpper(CultureInfo.CurrentCulture);

                case StringCaseType.Title:
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    return textInfo.ToTitleCase(value.ToLower(CultureInfo.CurrentCulture));

                default:
                    return value;
            }
        }
    }
}