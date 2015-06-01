namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Formats a number as a string
    /// </summary>
    [DataContract(Name = "format-number", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Format number")]
    public class FormatNumberTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the FormatNumberTransform class
        /// </summary>
        public FormatNumberTransform()
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
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Gets or sets the format string to apply to the number by this transform
        /// </summary>
        [DataMember(Name = "format")]
        public string Format { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.ApplyFormat(TypeConverter.ConvertData<long>(inputValue));
        }

        /// <summary>
        /// Applies the format to the number
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The string representation of the formatted number</returns>
        private object ApplyFormat(long value)
        {
            return value.ToString(this.Format);
        }
    }
}