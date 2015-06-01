namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Converts between binary SIDs and SID strings
    /// </summary>
    [DataContract(Name = "sid-string-bidi", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("SID to string (bi-directional)")]
    public class SidStringBiDirectionalTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the SidStringBiDirectionalTransform class
        /// </summary>
        public SidStringBiDirectionalTransform()
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
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Undefined;
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
            }
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            if (inputValue is string)
            {
                return this.ConvertSidStringToSid(TypeConverter.ConvertData<string>(inputValue));
            }
            else if (inputValue is byte[])
            {
                return this.ConvertSidToSidString(TypeConverter.ConvertData<byte[]>(inputValue));
            }
            else
            {
                throw new UnknownOrUnsupportedDataTypeException("Unsupported input format");
            }
        }

        /// <summary>
        /// Converts a SID string to its binary format
        /// </summary>
        /// <param name="value">The string representation of the SID (starting with S-)</param>
        /// <returns>The binary representation of the SID</returns>
        private object ConvertSidStringToSid(string value)
        {
            if (value.StartsWith("s-", StringComparison.OrdinalIgnoreCase))
            {
                return Utils.ConvertStringToSid(value);
            }
            else
            {
                try
                {
                    byte[] binaryValue = Convert.FromBase64String(value);
                    return this.ConvertSidToSidString(binaryValue);
                }
                catch (Exception ex)
                {
                    throw new UnknownOrUnsupportedDataTypeException("The input value was not a SID string or a base 64 encoded value", ex);
                }
            }
        }

        /// <summary>
        /// Converts a binary SID to its string format
        /// </summary>
        /// <param name="value">The binary representation of the SID</param>
        /// <returns>The string representation of the SID</returns>
        private object ConvertSidToSidString(byte[] value)
        {
            return Utils.ConvertSidToString(value);
        }
    }
}