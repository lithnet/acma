namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Performs a cryptographic hash of an incoming value
    /// </summary>
    [DataContract(Name = "hash", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Cryptographic hash")]
    public class HashTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the HashTransform class
        /// </summary>
        public HashTransform()
        {
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.Binary;
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
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Gets or sets the type of hash to apply to the incoming value
        /// </summary>
        [DataMember(Name = "hash-type")]
        public HashType HashType { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.HashValue(inputValue);
        }

        /// <summary>
        /// Hashes the value using the specified hash type
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private byte[] HashValue(object value)
        {
            switch (this.HashType)
            {
                case HashType.Sha1:
                    return this.HashSha1(value);

                case HashType.Sha256:
                    return this.HashSha256(value);

                case HashType.Sha384:
                    return this.HashSha384(value);

                case HashType.Sha512:
                    return this.HashSha512(value);

                default:
                    throw new InvalidOperationException("The specified hash type is unknown");
            }
        }

        /// <summary>
        /// Hashes the value using the SHA1 hash algorithm
        /// </summary>
        /// <param name="value">The string, integer, or binary value to hash</param>
        /// <returns>The hashed value</returns>
        private byte[] HashSha1(object value)
        {
            SHA1Managed hash = new SHA1Managed();
            byte[] input = this.GetInputBytes(value);
            byte[] result = hash.ComputeHash(input);

            return result;
        }

        /// <summary>
        /// Hashes the value using the SHA256 hash algorithm
        /// </summary>
        /// <param name="value">The string, integer, or binary value to hash</param>
        /// <returns>The hashed value</returns>
        private byte[] HashSha256(object value)
        {
            SHA256Managed hash = new SHA256Managed();
            byte[] input = this.GetInputBytes(value);
            byte[] result = hash.ComputeHash(input);

            return result;
        }

        /// <summary>
        /// Hashes the value using the SHA384 hash algorithm
        /// </summary>
        /// <param name="value">The string, integer, or binary value to hash</param>
        /// <returns>The hashed value</returns>
        private byte[] HashSha384(object value)
        {
            SHA384Managed hash = new SHA384Managed();
            byte[] input = this.GetInputBytes(value);
            byte[] result = hash.ComputeHash(input);

            return result;
        }

        /// <summary>
        /// Hashes the value using the SHA512 hash algorithm
        /// </summary>
        /// <param name="value">The string, integer, or binary value to hash</param>
        /// <returns>The hashed value</returns>
        private byte[] HashSha512(object value)
        {
            SHA512Managed hash = new SHA512Managed();
            byte[] input = this.GetInputBytes(value);
            byte[] result = hash.ComputeHash(input);

            return result;
        }

        /// <summary>
        /// Converts the input string, integer or binary value to its byte[] representation
        /// </summary>
        /// <param name="input">A string, integer, or binary value</param>
        /// <returns>A byte array representing the input value</returns>
        private byte[] GetInputBytes(object input)
        {
            if (input == null)
            {
                return null;
            }

            if (input is byte[])
            {
                return input as byte[];
            }
            else if (input is string)
            {
                string inputString = input as string;

                if (string.IsNullOrWhiteSpace(inputString))
                {
                    return null;
                }

                return UTF8Encoding.UTF8.GetBytes(inputString);
            }
            else if (input is long || input is int)
            {
                long inputInt = (long)input;
                if (inputInt == 0)
                {
                    return null;
                }

                return BitConverter.GetBytes(inputInt);
            }
            else
            {
                throw new InvalidOperationException("The data type is unknown");
            }
        }
    }
}