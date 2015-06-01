namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Selects a single value from a multivalued list
    /// </summary>
    [DataContract(Name = "mv-to-sv", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Multivalued to single-valued")]
    [HandlesOwnMultivaluedInput]
    public class MultivalueToSingleValueTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the MultivalueToSingleValueTransform class
        /// </summary>
        public MultivalueToSingleValueTransform()
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
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.String;

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
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Reference;

                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return ExtendedAttributeType.DateTime;
                }
            }
        }

        /// <summary>
        /// Gets or sets the to operator to apply to the source value
        /// </summary>
        [DataMember(Name = "operator")]
        public ValueOperator SelectorOperator { get; set; }

        /// <summary>
        /// Gets or sets the comparison type to use
        /// </summary>
        [DataMember(Name = "compare-as")]
        public ExtendedAttributeType CompareAs { get; set; }

        /// <summary>
        /// Gets or sets the value to use when comparing the source value with the selector operator
        /// </summary>
        [DataMember(Name = "value")]
        public string SelectorValue { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.GetSingleValue(new List<object>() { inputValue });
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="inputValues">The incoming values to transform</param>
        /// <returns>The transformed values</returns>
        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            object result = this.GetSingleValue(inputValues);

            if (result != null)
            {
                return new List<object>() { result };
            }
            else
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="inputValues">The values to find a match within</param>
        /// <returns>An attribute value</returns>
        private object GetSingleValue(IList<object> inputValues)
        {
            try
            {
                if (this.SelectorOperator == ValueOperator.First)
                {
                    object result = inputValues.FirstOrDefault(t => t != null);
                    return result == null ? null : TypeConverter.ConvertData(result, this.CompareAs);
                }
                else if (this.SelectorOperator == ValueOperator.Last)
                {
                    object result = inputValues.LastOrDefault(t => t != null);
                    return result == null ? null : TypeConverter.ConvertData(result, this.CompareAs);
                }

                switch (this.CompareAs)
                {
                    case ExtendedAttributeType.Binary:
                        return this.GetValueFromMvBinary(inputValues);

                    case ExtendedAttributeType.Integer:
                        return this.GetValueFromMvInteger(inputValues);

                    case ExtendedAttributeType.DateTime:
                        return this.GetValueFromMvDateTime(inputValues);

                    case ExtendedAttributeType.Reference:
                        return this.GetValueFromMvReference(inputValues);

                    case ExtendedAttributeType.String:
                        return this.GetValueFromMvString(inputValues);

                    case ExtendedAttributeType.Undefined:
                    case ExtendedAttributeType.Boolean:
                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }
            catch (NotFoundException)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="source">The source attribute array</param>
        /// <returns>An attribute value</returns>
        private byte[] GetValueFromMvBinary(IList<object> source)
        {
            foreach (object value in source)
            {
                byte[] valueByte = TypeConverter.ConvertData<byte[]>(value);

                if (ComparisonEngine.CompareBinary(valueByte, this.SelectorValue, this.SelectorOperator))
                {
                    return valueByte;
                }
            }

            throw new NotFoundException();
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="source">The source attribute array</param>
        /// <returns>An attribute value</returns>
        private string GetValueFromMvString(IList<object> source)
        {
            foreach (object value in source)
            {
                string valueString = TypeConverter.ConvertData<string>(value);

                if (ComparisonEngine.CompareString(valueString, this.SelectorValue, this.SelectorOperator))
                {
                    return valueString;
                }
            }

            throw new NotFoundException();
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="source">The source attribute array</param>
        /// <returns>An attribute value</returns>
        private Guid GetValueFromMvReference(IList<object> source)
        {
            foreach (object value in source)
            {
                Guid valueGuid = TypeConverter.ConvertData<Guid>(value);

                if (ComparisonEngine.CompareString(valueGuid.ToString(), this.SelectorValue, this.SelectorOperator))
                {
                    return valueGuid;
                }
            }

            throw new NotFoundException();
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="source">The source attribute array</param>
        /// <returns>An attribute value</returns>
        private long GetValueFromMvInteger(IList<object> source)
        {
            long? lastValue = null;

            foreach (object value in source)
            {
                long valueLong = TypeConverter.ConvertData<long>(value);

                switch (this.SelectorOperator)
                {
                    case ValueOperator.Equals:
                    case ValueOperator.NotEquals:
                    case ValueOperator.GreaterThan:
                    case ValueOperator.LessThan:
                    case ValueOperator.GreaterThanOrEq:
                    case ValueOperator.LessThanOrEq:
                    case ValueOperator.And:
                    case ValueOperator.Or:
                        if (ComparisonEngine.CompareLong(valueLong, this.SelectorValue, this.SelectorOperator))
                        {
                            return valueLong;
                        }

                        break;

                    case ValueOperator.Largest:
                        if (!lastValue.HasValue || valueLong > lastValue)
                        {
                            lastValue = valueLong;
                        }

                        break;

                    case ValueOperator.Smallest:
                        if (!lastValue.HasValue || valueLong < lastValue)
                        {
                            lastValue = valueLong;
                        }

                        break;

                    case ValueOperator.Contains:
                    case ValueOperator.NotContains:
                    case ValueOperator.StartsWith:
                    case ValueOperator.EndsWith:
                    case ValueOperator.IsPresent:
                    case ValueOperator.NotPresent:
                    default:
                        throw new UnknownOrUnsupportedValueOperatorException(this.SelectorOperator, typeof(long));
                }
            }

            if (lastValue.HasValue)
            {
                return lastValue.Value;
            }

            throw new NotFoundException();
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="source">The source attribute array</param>
        /// <returns>An attribute value</returns>
        private DateTime GetValueFromMvDateTime(IList<object> source)
        {
            DateTime? lastValue = null;

            foreach (object value in source)
            {
                DateTime valueDateTime = TypeConverter.ConvertData<DateTime>(value);

                switch (this.SelectorOperator)
                {
                    case ValueOperator.Equals:
                    case ValueOperator.NotEquals:
                    case ValueOperator.GreaterThan:
                    case ValueOperator.LessThan:
                    case ValueOperator.GreaterThanOrEq:
                    case ValueOperator.LessThanOrEq:
                    case ValueOperator.And:
                    case ValueOperator.Or:
                        if (ComparisonEngine.CompareDateTime(valueDateTime, DateTime.Parse(this.SelectorValue), this.SelectorOperator))
                        {
                            return valueDateTime;
                        }

                        break;

                    case ValueOperator.Largest:
                        if (!lastValue.HasValue || valueDateTime > lastValue)
                        {
                            lastValue = valueDateTime;
                        }

                        break;

                    case ValueOperator.Smallest:
                        if (!lastValue.HasValue || valueDateTime < lastValue)
                        {
                            lastValue = valueDateTime;
                        }

                        break;

                    case ValueOperator.Contains:
                    case ValueOperator.NotContains:
                    case ValueOperator.StartsWith:
                    case ValueOperator.EndsWith:
                    case ValueOperator.IsPresent:
                    case ValueOperator.NotPresent:
                    default:
                        throw new UnknownOrUnsupportedValueOperatorException(this.SelectorOperator, typeof(long));
                }
            }

            if (lastValue.HasValue)
            {
                return lastValue.Value;
            }

            throw new NotFoundException();
        }
    }
}