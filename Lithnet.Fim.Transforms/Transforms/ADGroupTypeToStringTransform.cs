namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Converts a flag from the group type integer into its corresponding string name
    /// </summary>
    [DataContract(Name = "ad-group-type-to-string", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("AD group type to string")]
    [MultivaluedInputNotSupported]
    public class ADGroupTypeToStringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the ADGroupTypeToStringTransform class
        /// </summary>
        public ADGroupTypeToStringTransform()
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
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.GetFlagName(TypeConverter.ConvertData<long>(inputValue));
        }
        
        /// <summary>
        /// Converts the flag value to its specified name
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private object GetFlagName(long value)
        {
            return (value & 0x80000000) == 0x80000000 ? "Security" : "Distribution";
        }
    }
}