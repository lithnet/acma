// -----------------------------------------------------------------------
// <copyright file="AttributeValue.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Linq;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Fim.Core;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Contains the value of an attribute and methods for accessing its strongly-typed value
    /// </summary>
    public class AttributeValue
    {
        /// <summary>
        /// The schema attribute associated with the value stored in this object
        /// </summary>
        private AcmaSchemaAttribute attribute;

        /// <summary>
        /// The raw object value
        /// </summary>
        private object internalValue;

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="attribute">The schema attribute associated with the value stored in this object</param>
        /// <param name="value">The value to assign this object</param>
        public AttributeValue(AcmaSchemaAttribute attribute, object value)
        {
            this.attribute = attribute;
            this.internalValue = TypeConverter.ConvertData(value, attribute.Type);
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="attribute">The schema attribute associated with the value stored in this object</param>
        public AttributeValue(AcmaSchemaAttribute attribute)
        {
            this.attribute = attribute;
        }

        /// <summary>
        /// Gets the attribute associated with this object
        /// </summary>
        public AcmaSchemaAttribute Attribute
        {
            get
            {
                return this.attribute;
            }
        }

        /// <summary>
        /// Gets the underlying object value cast as a string
        /// </summary>
        public string ValueString
        {
            get
            {
                return (string)this.Value;
            }
        }

        /// <summary>
        /// Gets the underlying object value cast as a byte array
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte array")]
        public byte[] ValueByte
        {
            get
            {
                return (byte[])this.Value;
            }
        }

        /// <summary>
        /// Gets the underlying object value cast as a long integer
        /// </summary>
        public long ValueLong
        {
            get
            {
                return (long)this.Value;
            }
        }

        /// <summary>
        /// Gets the underlying object value cast as a long integer
        /// </summary>
        public DateTime ValueDateTime
        {
            get
            {
                return (DateTime)this.Value;
            }
        }

        /// <summary>
        /// Gets the underlying object value cast as a GUID
        /// </summary>
        public Guid ValueGuid
        {
            get
            {
                if (this.Value == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return (Guid)this.Value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the underlying object value cast as a boolean value is true
        /// </summary>
        public bool ValueBoolean
        {
            get
            {
                return this.Value == null ? false : (bool)this.Value;
            }
        }

        /// <summary>
        /// Gets the underlying object value
        /// </summary>
        public virtual object Value
        {
            get
            {
                return this.internalValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the underlying value is null
        /// </summary>
        public virtual bool IsNull
        {
            get
            {
                return this.Value == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the underlying value has changed
        /// </summary>
        public virtual bool HasChanged
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if an AttributeValue object has the same underlying value as another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public static bool operator ==(AttributeValue x, string y)
        {
            return x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has the same underlying value as another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public static bool operator ==(AttributeValue x, long y)
        {
            return x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has the same underlying value as another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public static bool operator ==(AttributeValue x, byte[] y)
        {
            return x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has the same underlying value as another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public static bool operator ==(AttributeValue x, bool y)
        {
            return x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has the same underlying value as another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public static bool operator ==(AttributeValue x, object y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has the same underlying value as another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public static bool operator ==(AttributeValue x, AttributeValue y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has a different underlying value from another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are not the same</returns>
        public static bool operator !=(AttributeValue x, string y)
        {
            return !x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has a different underlying value from another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are not the same</returns>
        public static bool operator !=(AttributeValue x, byte[] y)
        {
            return !x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has a different underlying value from another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are not the same</returns>
        public static bool operator !=(AttributeValue x, bool y)
        {
            return !x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has a different underlying value from another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are not the same</returns>
        public static bool operator !=(AttributeValue x, long y)
        {
            return !x.ValueEquals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has a different underlying value from another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are not the same</returns>
        public static bool operator !=(AttributeValue x, AttributeValue y)
        {
            return !x.Equals(y);
        }

        /// <summary>
        /// Determines if an AttributeValue object has a different underlying value from another object
        /// </summary>
        /// <param name="x">The AttributeValue to compare</param>
        /// <param name="y">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are not the same</returns>
        public static bool operator !=(AttributeValue x, object y)
        {
            return !x.Equals(y);
        }

        /// <summary>
        /// Determines if this AttributeValue object has the same value as another object
        /// </summary>
        /// <param name="obj">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (Object.ReferenceEquals(obj, this))
            {
                return true;
            }
            else
            {
                return this.ValueEquals(obj);
            }
        }

        /// <summary>
        /// Determines if this AttributeValue object has the same value as another object
        /// </summary>
        /// <param name="obj">The value to compare</param>
        /// <returns>A boolean value indicating if the two values are the same</returns>
        public bool ValueEquals(object obj)
        {
            if (this.Value == null && obj == null)
            {
                return true;
            }

            if (this.Value == null ^ obj == null)
            {
                return false;
            }

            object comparisonValue;

            if (obj is AttributeValue)
            {
                AttributeValue otherValue = (AttributeValue)obj;

                if (this.IsNull && otherValue.IsNull)
                {
                    //// If both values are null
                    return true;
                }
                else if (this.IsNull || otherValue.IsNull)
                {
                    //// If one of the two values are null
                    return false;
                }
                else if (this.Attribute != otherValue.Attribute)
                {
                    //// If the types don't match
                    return false;
                }

                comparisonValue = otherValue.Value;
            }
            else
            {
                comparisonValue = obj;
            }

            switch (this.attribute.Type)
            {
                case ExtendedAttributeType.Boolean:
                case ExtendedAttributeType.Integer:
                case ExtendedAttributeType.DateTime:
                case ExtendedAttributeType.Reference:
                case ExtendedAttributeType.Binary:
                    return ComparisonEngine.Compare(comparisonValue, this.Value, ValueOperator.Equals, this.Attribute.Type);

                case ExtendedAttributeType.String:
                    return ComparisonEngine.CompareString(TypeConverter.ConvertData<string>(comparisonValue), this.ValueString, ValueOperator.Equals, StringComparison.CurrentCulture);

                case ExtendedAttributeType.Undefined:
                default:
                    throw new ArgumentException("Unknown or unsupported attribute type");
            }
        }

        /// <summary>
        /// Serves as a hash function for a particular type
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return this.attribute.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.internalValue == null ? null : this.internalValue.ToString();
        }
    }
}