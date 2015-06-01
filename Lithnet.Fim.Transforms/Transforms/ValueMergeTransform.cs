namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Combines multiple incoming values into a single value collection
    /// </summary>
    [DataContract(Name = "value-merge", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Value merge transform")]
    [HandlesOwnMultivaluedInput]
    public class ValueMergeTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the ValueMergeTransform class
        /// </summary>
        public ValueMergeTransform()
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
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.Reference;
                
                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return ExtendedAttributeType.DateTime;
                }
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
                yield return ExtendedAttributeType.Reference;

                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return ExtendedAttributeType.DateTime;
                }
            }
        }

        /// <summary>
        /// Gets or sets the data type that this transform should return the result as 
        /// </summary>
        [DataMember(Name = "return-type")]
        public ExtendedAttributeType UserDefinedReturnType { get; set; }

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

            return TypeConverter.ConvertData(inputValue, this.UserDefinedReturnType);
        }

        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            switch (this.UserDefinedReturnType)
            {
                case ExtendedAttributeType.String:
                    return this.MergeAs<string>(inputValues);

                case ExtendedAttributeType.Integer:
                    return this.MergeAs<long>(inputValues);

                case ExtendedAttributeType.Reference:
                    return this.MergeAs<Guid>(inputValues);

                case ExtendedAttributeType.Binary:
                    return this.MergeAs<byte[]>(inputValues);

                case ExtendedAttributeType.DateTime:
                    return this.MergeAs<DateTime>(inputValues);

                case ExtendedAttributeType.Undefined:
                case ExtendedAttributeType.Boolean:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        private IList<object> MergeAs<T>(IList<object> inputValues)
        {
            List<T> castValues = new List<T>();

            castValues.AddRange(inputValues.Select(t => TypeConverter.ConvertData<T>(t)));

            return castValues.Distinct().Cast<object>().ToList();
        }
    }
}