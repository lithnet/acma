// -----------------------------------------------------------------------
// <copyright file="ComparisonEngine.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Provides methods for comparing values
    /// </summary>
    public static class ComparisonEngine
    {
        public static IEnumerable<ValueOperator> GetAllowedValueOperators(AttributeType type)
        {
            return ComparisonEngine.GetAllowedValueOperators(type.ToExtendedAttributeType());
        }

        public static IEnumerable<ValueOperator> GetAllowedValueOperators(ExtendedAttributeType type)
        {
            switch (type)
            {
                case ExtendedAttributeType.Binary:
                    return ComparisonEngine.AllowedBinaryOperators;

                case ExtendedAttributeType.Boolean:
                    return ComparisonEngine.AllowedBooleanOperators;

                case ExtendedAttributeType.Integer:
                    return ComparisonEngine.AllowedIntegerOperators;

                case ExtendedAttributeType.DateTime:
                    return ComparisonEngine.AllowedDateTimeOperators;

                case ExtendedAttributeType.Reference:
                    return ComparisonEngine.AllowedReferenceOperators;

                case ExtendedAttributeType.String:
                    return ComparisonEngine.AllowedStringOperators;

                case ExtendedAttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        public static bool IsAllowedOperator(ValueOperator op, AttributeType type)
        {
            return ComparisonEngine.IsAllowedOperator(op, type.ToExtendedAttributeType());
        }

        public static bool IsAllowedOperator(ValueOperator op, ExtendedAttributeType type)
        {
            switch (type)
            {
                case ExtendedAttributeType.Binary:
                    return ComparisonEngine.AllowedBinaryOperators.Any(t => t == op);

                case ExtendedAttributeType.Boolean:
                    return ComparisonEngine.AllowedBooleanOperators.Any(t => t == op);

                case ExtendedAttributeType.Integer:
                    return ComparisonEngine.AllowedIntegerOperators.Any(t => t == op);

                case ExtendedAttributeType.DateTime:
                    return ComparisonEngine.AllowedDateTimeOperators.Any(t => t == op);

                case ExtendedAttributeType.Reference:
                    return ComparisonEngine.AllowedReferenceOperators.Any(t => t == op);

                case ExtendedAttributeType.String:
                    return ComparisonEngine.AllowedStringOperators.Any(t => t == op);

                case ExtendedAttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a string value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedStringOperators
        {
            get
            {
                yield return ValueOperator.Equals;
                yield return ValueOperator.NotEquals;
                yield return ValueOperator.Contains;
                yield return ValueOperator.NotContains;
                yield return ValueOperator.StartsWith;
                yield return ValueOperator.EndsWith;
            }
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a numeric value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedIntegerOperators
        {
            get
            {
                yield return ValueOperator.Equals;
                yield return ValueOperator.NotEquals;
                yield return ValueOperator.GreaterThan;
                yield return ValueOperator.LessThan;
                yield return ValueOperator.GreaterThanOrEq;
                yield return ValueOperator.LessThanOrEq;
            }
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a DateTime value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedDateTimeOperators
        {
            get
            {
                yield return ValueOperator.Equals;
                yield return ValueOperator.NotEquals;
                yield return ValueOperator.GreaterThan;
                yield return ValueOperator.LessThan;
                yield return ValueOperator.GreaterThanOrEq;
                yield return ValueOperator.LessThanOrEq;
            }
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a binary value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedBinaryOperators
        {
            get
            {
                yield return ValueOperator.Equals;
                yield return ValueOperator.NotEquals;
            }
        }

        /// <summary>
        /// Gets the PresenceOperators that are allowed to be used with an object value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedPresenceOperators
        {
            get
            {
                yield return ValueOperator.IsPresent;
                yield return ValueOperator.NotPresent;
            }
        }

        /// <summary>
        /// Gets the ValueOperator that are allowed to be used with a state value
        /// </summary>
        /// <returns>An array of allowed ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedBooleanOperators
        {
            get
            {
                yield return ValueOperator.Equals;
                yield return ValueOperator.NotEquals;
            }
        }

        /// <summary>
        /// Gets the ValueOperator that are allowed to be used with a state value
        /// </summary>
        /// <returns>An array of allowed ValueOperator values</returns>
        public static IEnumerable<ValueOperator> AllowedReferenceOperators
        {
            get
            {
                yield return ValueOperator.Equals;
                yield return ValueOperator.NotEquals;
            }
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a state value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedBooleanOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedBooleanOperators.Any(t => t == valueOperator);
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a string value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedStringOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedStringOperators.Any(t => t == valueOperator);
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a numeric value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedIntegerOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedIntegerOperators.Any(t => t == valueOperator);
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a DateTime value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedDateTimeOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedDateTimeOperators.Any(t => t == valueOperator);
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a binary value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedBinaryOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedBinaryOperators.Any(t => t == valueOperator);
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a binary value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedPresenceOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedPresenceOperators.Any(t => t == valueOperator);
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a binary value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedReferenceOperator(ValueOperator valueOperator)
        {
            return ComparisonEngine.AllowedReferenceOperators.Any(t => t == valueOperator);
        }

        public static bool Compare(object actualValue, object expectedValue, ValueOperator valueOperator, AttributeType type)
        {
            return ComparisonEngine.Compare(actualValue, expectedValue, valueOperator, type.ToExtendedAttributeType());
        }

        public static bool Compare(object actualValue, object expectedValue, ValueOperator valueOperator, ExtendedAttributeType type)
        {
            switch (type)
            {
                case ExtendedAttributeType.Binary:
                    return CompareBinary(TypeConverter.ConvertData<byte[]>(actualValue),
                        TypeConverter.ConvertData<byte[]>(expectedValue),
                        valueOperator);

                case ExtendedAttributeType.Boolean:
                    return CompareBoolean(TypeConverter.ConvertData<bool>(actualValue),
                       TypeConverter.ConvertData<bool>(expectedValue),
                       valueOperator);

                case ExtendedAttributeType.Integer:
                    return CompareLong(TypeConverter.ConvertData<long>(actualValue),
                         TypeConverter.ConvertData<long>(expectedValue),
                         valueOperator);

                case ExtendedAttributeType.DateTime:
                    return CompareDateTime(TypeConverter.ConvertData<DateTime>(actualValue),
                         TypeConverter.ConvertData<DateTime>(expectedValue),
                         valueOperator);

                case ExtendedAttributeType.Reference:
                    return CompareString(TypeConverter.ConvertData<Guid>(actualValue).ToString(),
                        TypeConverter.ConvertData<Guid>(expectedValue).ToString(),
                        valueOperator);

                case ExtendedAttributeType.String:
                    return CompareString(TypeConverter.ConvertData<string>(actualValue),
                         TypeConverter.ConvertData<string>(expectedValue),
                         valueOperator);

                default:
                case ExtendedAttributeType.Undefined:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

         /// <summary>
        /// Compares a string using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareString(string actualValue, string expectedValue, ValueOperator valueOperator)
        {
            return ComparisonEngine.CompareString(actualValue, expectedValue, valueOperator, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Compares a string using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareString(string actualValue, string expectedValue, ValueOperator valueOperator, StringComparison comparisonType)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (string.Equals(actualValue, expectedValue, comparisonType))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (!string.Equals(actualValue, expectedValue, comparisonType))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.IsPresent:
                    return !string.IsNullOrEmpty(actualValue);

                case ValueOperator.NotPresent:
                    return string.IsNullOrEmpty(actualValue);

                case ValueOperator.Contains:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.IndexOf(expectedValue, comparisonType) != -1)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotContains:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.IndexOf(expectedValue, comparisonType) == -1)
                    {
                        return true;
                    }

                    break;
                case ValueOperator.StartsWith:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.StartsWith(expectedValue, comparisonType))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.EndsWith:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.EndsWith(expectedValue, comparisonType))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                case ValueOperator.LessThan:
                case ValueOperator.GreaterThanOrEq:
                case ValueOperator.LessThanOrEq:
                default:
                    throw new UnknownOrUnsupportedValueOperatorException(valueOperator, typeof(string));
            }

            return false;
        }

        /// <summary>
        /// Compares a binary value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="base64ExpectedValue">The expected value specified in the query logic, in base64 format</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBinary(byte[] actualValue, string base64ExpectedValue, ValueOperator valueOperator)
        {
            byte[] expectedValue = Convert.FromBase64String(base64ExpectedValue);
            return CompareBinary(actualValue, expectedValue, valueOperator);
        }

        /// <summary>
        /// Compares a binary value using the specified ValueOperator
        /// </summary>
        /// <param name="base64ActualValue">The value obtained from the query</param>
        /// <param name="base64ExpectedValue">The expected value specified in the query logic, in base64 format</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBinary(string base64ActualValue, string base64ExpectedValue, ValueOperator valueOperator)
        {
            byte[] expectedValue = Convert.FromBase64String(base64ExpectedValue);
            byte[] actualValue = Convert.FromBase64String(base64ActualValue);

            return CompareBinary(actualValue, expectedValue, valueOperator);
        }

        /// <summary>
        /// Compares a binary using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBinary(byte[] actualValue, byte[] expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == null && expectedValue == null)
                    {
                        return true;
                    }
                    else if (actualValue == null || expectedValue == null)
                    {
                        return false;
                    }
                    else if (actualValue.SequenceEqual(expectedValue))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue == null && expectedValue == null)
                    {
                        return false;
                    }
                    else if (actualValue == null || expectedValue == null)
                    {
                        return true;
                    }
                    else if (!actualValue.SequenceEqual(expectedValue))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.IsPresent:
                    if ((actualValue != null) && (actualValue.Length > 0))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotPresent:
                    if ((actualValue == null) || (actualValue.Length == 0))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                case ValueOperator.LessThan:
                case ValueOperator.GreaterThanOrEq:
                case ValueOperator.LessThanOrEq:
                case ValueOperator.Contains:
                case ValueOperator.StartsWith:
                case ValueOperator.EndsWith:
                default:
                    throw new UnknownOrUnsupportedValueOperatorException(valueOperator, typeof(byte[]));
            }

            return false;
        }

        /// <summary>
        /// Compares a numeric value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareLong(string actualValue, string expectedValue, ValueOperator valueOperator)
        {
            long expectedValueConverted = Convert.ToInt64(expectedValue);
            long actualValueConverted = Convert.ToInt64(actualValue);
            return CompareLong(actualValueConverted, expectedValueConverted, valueOperator);
        }

        /// <summary>
        /// Compares a numeric value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareLong(long actualValue, string expectedValue, ValueOperator valueOperator)
        {
            long expectedValueConverted = Convert.ToInt64(expectedValue);
            return CompareLong(actualValue, expectedValueConverted, valueOperator);
        }

        /// <summary>
        /// Compares a numeric value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareLong(long actualValue, long expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue != expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                    if (actualValue > expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.LessThan:
                    if (actualValue < expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThanOrEq:
                    if (actualValue >= expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.LessThanOrEq:
                    if (actualValue <= expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.Or:
                    if ((actualValue | expectedValue) == actualValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.And:
                    if ((actualValue & expectedValue) == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.IsPresent:
                case ValueOperator.NotPresent:
                case ValueOperator.Contains:
                case ValueOperator.StartsWith:
                case ValueOperator.EndsWith:
                default:
                    throw new UnknownOrUnsupportedValueOperatorException(valueOperator, typeof(long));
            }

            return false;
        }

        /// <summary>
        /// Compares a DateTime value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareDateTime(DateTime actualValue, DateTime expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue != expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                    if (actualValue > expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.LessThan:
                    if (actualValue < expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThanOrEq:
                    if (actualValue >= expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.LessThanOrEq:
                    if (actualValue <= expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.IsPresent:
                case ValueOperator.NotPresent:
                case ValueOperator.Contains:
                case ValueOperator.StartsWith:
                case ValueOperator.EndsWith:
                default:
                    throw new UnknownOrUnsupportedValueOperatorException(valueOperator, typeof(long));
            }

            return false;
        }

        /// <summary>
        /// Compares a boolean value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBoolean(string actualValue, string expectedValue, ValueOperator valueOperator)
        {
            bool expectedValueConverted = Convert.ToBoolean(expectedValue);
            bool actualValueConverted = Convert.ToBoolean(actualValue);
            return CompareBoolean(actualValueConverted, expectedValueConverted, valueOperator);
        }

        /// <summary>
        /// Compares a boolean value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBoolean(bool actualValue, string expectedValue, ValueOperator valueOperator)
        {
            bool expectedValueConverted = Convert.ToBoolean(expectedValue);
            return CompareBoolean(actualValue, expectedValueConverted, valueOperator);
        }

        /// <summary>
        /// Compares a boolean value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBoolean(bool actualValue, bool expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue != expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                case ValueOperator.LessThan:
                case ValueOperator.GreaterThanOrEq:
                case ValueOperator.LessThanOrEq:
                case ValueOperator.IsPresent:
                case ValueOperator.NotPresent:
                case ValueOperator.Contains:
                case ValueOperator.StartsWith:
                case ValueOperator.EndsWith:
                default:
                    throw new UnknownOrUnsupportedValueOperatorException(valueOperator, typeof(bool));
            }

            return false;
        }
    }
}
