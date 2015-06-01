namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Converts a set of group strings to the appropriate AD groupType flag values
    /// </summary>
    [DataContract(Name = "group-strings-to-ad-group-type", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Portal group strings to AD groupType")]
    [HandlesOwnMultivaluedInput]
    public class GroupStringsToADGroupTypeTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the GroupStringsToADGroupTypeTransform class
        /// </summary>
        public GroupStringsToADGroupTypeTransform()
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
                yield return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            switch (inputValue.ToSmartStringOrEmptyString().ToLower())
            {
                case "security":
                case "mailenabledsecurity":
                    return (long)unchecked((int)0x80000002); // returning 2 because a group must have a scope

                case "distribution":
                    return 2L; // returning 2 because a group must have a scope

                case "global":
                    return 2L;

                case "domainlocal":
                    return 4L;

                case "universal":
                    return 8L;

                default:
                    throw new ArgumentException("The input values were not of the expected type. Allowed values are: Security, Distribution, DomainLocal, Universal, Global");
            }
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="inputValues">The incoming values to transform</param>
        /// <returns>The transformed values</returns>
        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            if (inputValues.Count > 2)
            {
                throw new TooManyValuesException("The transform can only accept no more than two input values");
            }

            bool isSecurity = false;
            bool isDistribution = false;
            bool isGlobal = false;
            bool isDomainLocal = false;
            bool isUniversal = false;

            foreach (string inputValue in inputValues.Select(t => t.ToSmartStringOrEmptyString()))
            {
                switch (inputValue.ToLowerInvariant())
                {
                    case "security":
                    case "mailenabledsecurity":
                        if (isDistribution)
                        {
                            throw new ArgumentException("A group cannot be both a security and distribution group");
                        }

                        isSecurity = true;
                        break;

                    case "distribution":
                        if (isSecurity)
                        {
                            throw new ArgumentException("A group cannot be both a security and distribution group");
                        }

                        isDistribution = true;
                        break;

                    case "global":
                        if (isDomainLocal || isUniversal)
                        {
                            throw new ArgumentException("A group cannot have more than one scope type specified");
                        }

                        isGlobal = true;
                        break;

                    case "domainlocal":
                        if (isGlobal || isUniversal)
                        {
                            throw new ArgumentException("A group cannot have more than one scope type specified");
                        }

                        isDomainLocal = true;
                        break;

                    case "universal":
                        if (isDomainLocal || isGlobal)
                        {
                            throw new ArgumentException("A group cannot have more than one scope type specified");
                        }

                        isUniversal = true;
                        break;

                    default:
                        throw new ArgumentException("The input values were not of the expected type. Allowed values are: Security, Distribution, DomainLocal, Universal, Global");
                }
            }

            if (!isSecurity && !isDistribution)
            {
                throw new ArgumentException("A group must have a type specified");
            }

            if (!isDomainLocal && !isGlobal && !isUniversal)
            {
                throw new ArgumentException("A group must have a scope type specified");
            }

            int flags = isSecurity ? unchecked((int)0x80000000) : 0;
            flags |= isGlobal ? 2 : 0;
            flags |= isDomainLocal ? 4 : 0;
            flags |= isUniversal ? 8 : 0;

            return new List<object>() { (long)flags };
        }
    }
}