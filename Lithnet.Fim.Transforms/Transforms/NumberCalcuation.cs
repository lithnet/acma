namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Performs a calculation on an integer value
    /// </summary>
    [DataContract(Name = "number-calculation", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Number calculation transform")]
    public class NumberCalculationTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the StringSplitTransform class
        /// </summary>
        public NumberCalculationTransform()
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
                yield return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Gets or sets the operation to apply to this value
        /// </summary>
        [DataMember(Name = "operator")]
        public NumberOperator Operator { get; set; }

        /// <summary>
        /// The value to apply to the calculation
        /// </summary>
        [DataMember(Name = "value")]
        public long Value { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            if (inputValue == null)
            {
                if (this.Operator == NumberOperator.DivideByIncoming)
                {
                    return 0;
                }

                inputValue = 0;
            }

            return this.GetSingleValue(TypeConverter.ConvertData<long>(inputValue));
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="inputValues">The values to find a match within</param>
        /// <returns>An attribute value</returns>
        private object GetSingleValue(long inputValue)
        {
            switch (this.Operator)
            {
                case NumberOperator.Add:
                    return this.Value + inputValue;

                case NumberOperator.SubtractFromTarget:
                    return this.Value - inputValue;

                case NumberOperator.SubtractFromIncoming:
                    return inputValue - this.Value;

                case NumberOperator.DivideByIncoming:
                    return this.Value / inputValue;

                case NumberOperator.DivideByTarget:
                    return inputValue / this.Value;

                case NumberOperator.Multiply:
                    return this.Value * inputValue;

                default:
                    throw new ArgumentException("Invalid operator value: " + this.Operator.ToSmartString());
            }
        }
    }
}