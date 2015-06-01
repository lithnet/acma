namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Formats a number as a string
    /// </summary>
    [DataContract(Name = "bitmask-to-boolean", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Bitmask to boolean")]
    [MultivaluedInputNotSupported]
    public class BitmaskToBooleanTransform : Transform
    {
        public BitmaskToBooleanTransform()
        {
        }

        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.Boolean;
            }
        }

        /// <summary>
        /// Gets or sets the format string to apply to the date by this transform
        /// </summary>
        [DataMember(Name = "flag")]
        public long Flag { get; set; }

        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputType">The type of data supplied as the input value</param>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.TransformValue(TypeConverter.ConvertData<long>(inputValue));
        }

        /// <summary>
        /// Performs the transformation against the specified value
        /// </summary>
        /// <param name="type">The type of data supplied as the input value</param>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private object TransformValue(long value)
        {
            return (value & this.Flag) > 0;
        }
    }
}