// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Ryan Newington">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.ComponentModel;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Defines extension methods used in the MA
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> strings)
        {
            string newString = string.Empty;

            foreach (string text in strings)
            {
                newString = newString.AppendWithCommaSeparator(text);
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToSeparatedString(this IEnumerable<string> strings, string seperator)
        {
            string newString = string.Empty;

            foreach (string text in strings)
            {
                newString = newString.AppendWithSeparator(seperator, text);
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToNewLineSeparatedString(this IEnumerable<string> strings)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string text in strings)
            {
                builder.AppendLine(text);
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithCommaSeparator(this string text, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += ", ";
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithSeparator(this string text, string seperator, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += seperator;
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object</returns>
        public static string ToSmartString(this object obj)
        {
            if (obj is byte[])
            {
                byte[] cast = (byte[])obj;
                return Convert.ToBase64String(cast);
            }
            else if (obj is long)
            {
                return ((long)obj).ToString();
            }
            else if (obj is string)
            {
                return ((string)obj).ToString();
            }
            else if (obj is bool)
            {
                return ((bool)obj).ToString();
            }
            else if (obj is Guid)
            {
                return ((Guid)obj).ToString();
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToString(TypeConverter.FimServiceDateFormat);
            }
            else if (obj == null)
            {
                return "null";
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrEmptyString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        public static string TruncateString(this string obj, int totalLength)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return obj;
            }

            if (totalLength <= 3)
            {
                throw new ArgumentException("totalLength", "The maxlength value must be greater than 3");
            }

            if (obj.Length > totalLength)
            {
                return obj.Substring(0, totalLength - 3) + "...";
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Gets a value indicating whether two enumerations contain the same elements, even if they are in different orders
        /// </summary>
        /// <typeparam name="T">The type of items in the enumerations</typeparam>
        /// <param name="enumeration1">The first list to compare</param>
        /// <param name="enumeration2">The second list to compare</param>
        /// <returns>A value indicating if the two enumerations contain the same objects</returns>
        public static bool ContainsSameElements<T>(this IEnumerable<T> enumeration1, IEnumerable<T> enumeration2)
        {
            List<T> list1 = enumeration1.ToList();
            List<T> list2 = enumeration2.ToList();

            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.Intersect(list2).Count() == list1.Count;
        }

        public static AttributeType ToMmsAttributeType(this ExtendedAttributeType type)
        {
            switch (type)
            {
                case ExtendedAttributeType.String:
                    return AttributeType.String;

                case ExtendedAttributeType.Integer:
                    return AttributeType.Integer;

                case ExtendedAttributeType.Reference:
                    return AttributeType.Reference;

                case ExtendedAttributeType.Binary:
                    return AttributeType.Binary;

                case ExtendedAttributeType.Boolean:
                    return AttributeType.Boolean;

                case ExtendedAttributeType.Undefined:
                    return AttributeType.Undefined;

                case ExtendedAttributeType.DateTime:
                    return AttributeType.String;

                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        public static ExtendedAttributeType ToExtendedAttributeType(this AttributeType type)
        {
            switch (type)
            {
                case AttributeType.Binary:
                    return ExtendedAttributeType.Binary;

                case AttributeType.Boolean:
                    return ExtendedAttributeType.Boolean;

                case AttributeType.Integer:
                    return ExtendedAttributeType.Integer;

                case AttributeType.Reference:
                    return ExtendedAttributeType.Reference;

                case AttributeType.String:
                    return ExtendedAttributeType.String;

                case AttributeType.Undefined:
                    return ExtendedAttributeType.Undefined;

                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        public static string ToFimServiceString(this DateTime dateTime)
        {
            return dateTime.ToString(TypeConverter.FimServiceDateFormat);
        }

        /// <summary>
        /// <para>Truncates a DateTime to a specified resolution.</para>
        /// <para>A convenient source for resolution is TimeSpan.TicksPerXXXX constants.</para>
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }
    }
}
