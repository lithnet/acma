namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Converts a flag from the group type integer into its corresponding string name
    /// </summary>
    [DataContract(Name = "ad-group-scope-to-string", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("AD group scope to string")]
    [MultivaluedInputNotSupported]
    public class ADGroupScopeToStringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the ADGroupScopeToStringTransform class
        /// </summary>
        public ADGroupScopeToStringTransform()
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
        /// Converts the flag value into its corresponding name
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private object GetFlagName(long value)
        {
            if ((value & 2) == 2)
            {
                return "Global";
            }
            else if ((value & 4) == 4)
            {
                return "DomainLocal";
            }
            else if ((value & 8) == 8)
            {
                return "Universal";
            }
            else
            {
                throw new InvalidOperationException("The groupType value could not be parsed as a valid scope: " + value.ToSmartString());
            }
        }
    }
}