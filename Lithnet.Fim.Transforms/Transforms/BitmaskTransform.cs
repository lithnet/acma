namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Applies a flag value to a integer value containing a bitmask
    /// </summary>
    [DataContract(Name = "bitmask", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Apply bitmask")]
    public class BitmaskTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the BitmaskTransform class
        /// </summary>
        public BitmaskTransform()
        {
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
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
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Gets or sets the flag value to apply to bitmask
        /// </summary>
        [DataMember(Name = "flag")]
        public long Flag { get; set; }

        /// <summary>
        /// Gets or sets the type of operation to perform against the target value
        /// </summary>
        [DataMember(Name = "operation")]
        public BitwiseOperation Operation { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.TransformValue(TypeConverter.ConvertData<long>(inputValue));
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private object TransformValue(long value)
        {
            long newValue;

            switch (this.Operation)
            {
                case BitwiseOperation.And:
                    newValue = value & this.Flag;
                    break;

                case BitwiseOperation.Or:
                    newValue = value | this.Flag;
                    break;

                case BitwiseOperation.Nand:
                    newValue = value & ~this.Flag;
                    break;

                case BitwiseOperation.Xor:
                    newValue = value ^ this.Flag;
                    break;

                default:
                    throw new ArgumentException("operator");
            }

            return newValue;
        }
    }
}