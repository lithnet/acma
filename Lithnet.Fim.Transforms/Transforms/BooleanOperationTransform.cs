namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Performs a logical boolean operation
    /// </summary>
    [DataContract(Name = "boolean-operation", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Logical boolean operation")]
    [HandlesOwnMultivaluedInput]
    public class BooleanOperationTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the BooleanOperationTransform class
        /// </summary>
        public BooleanOperationTransform()
        {
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.Boolean;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Gets or sets the logical operator with which to perform the calculation
        /// </summary>
        [DataMember(Name = "operator")]
        public BitwiseOperation Operator { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            if (inputValue == null)
            {
                return null;
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="inputValues">The incoming values to transform</param>
        /// <returns>The transformed values</returns>
        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            IList<bool> convertedValues = inputValues.Select(t => TypeConverter.ConvertData<bool>(t)).ToList();

            bool result = this.PerformLogicalComparison(convertedValues);

            return new List<object>() { result };
        }

        /// <summary>
        /// Performs the calculation using the classes boolean operator
        /// </summary>
        /// <param name="values">The incoming values to compare</param>
        /// <returns>The result of the boolean operation</returns>
        private bool PerformLogicalComparison(IList<bool> values)
        {
            switch (this.Operator)
            {
                case BitwiseOperation.And:
                    return values.All(t => t == true);

                case BitwiseOperation.Or:
                    return values.Any(t => t);

                case BitwiseOperation.Nand:
                    return values.All(t => !t);

                case BitwiseOperation.Xor:
                    return values.Count(t => t) == 1;

                default:
                    throw new ArgumentException("operator");
            }
        }
    }
}