namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Trims whitespace from a string
    /// </summary>
    [DataContract(Name = "trim-string", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Trim string")]
    public class TrimStringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the TrimStringTransform class
        /// </summary>
        public TrimStringTransform()
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
            }
        }

        /// <summary>
        /// Gets or sets the type of trim to perform
        /// </summary>
        [DataMember(Name = "trim-type")]
        public TrimType TrimType { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.TrimString(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Trims the specified string
        /// </summary>
        /// <param name="value">The string to trim</param>
        /// <returns>A trimmed string</returns>
        private object TrimString(string value)
        {
            string newString;

            switch (this.TrimType)
            {
                case TrimType.Left:
                    newString = value.TrimStart();
                    break;

                case TrimType.Right:
                    newString = value.TrimEnd();
                    break;

                case TrimType.Both:
                    newString = value.Trim();
                    break;

                case TrimType.None:
                default:
                    newString = value;
                    break;
            }

            return newString;
        }
    }
}