// -----------------------------------------------------------------------
// <copyright file="AttributeValues.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Linq;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Represents a collection of AttributeValue objects
    /// </summary>
    public abstract class AttributeValues : IEnumerable, IEnumerable<AttributeValue>
    {
        /// <summary>
        /// The schema attribute representing this value collection
        /// </summary>
        private AcmaSchemaAttribute attribute;

        /// <summary>
        /// Initializes a new instance of the AttributeValues class
        /// </summary>
        /// <param name="attribute">The schema attribute representing this value collection</param>
        public AttributeValues(AcmaSchemaAttribute attribute)
        {
            this.attribute = attribute;
            this.InternalValues = new List<AttributeValue>();
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValues class
        /// </summary>
        /// <param name="attribute">The schema attribute representing this value collection</param>
        /// <param name="values">The list of values to assign to this object</param>
        public AttributeValues(AcmaSchemaAttribute attribute, IList<object> values)
            : this(attribute)
        {
            if (values != null)
            {
                foreach (object value in values)
                {
                    this.InternalValues.Add(new AttributeValue(attribute, value));
                }
            }
        }

        public AttributeValues(AcmaSchemaAttribute attribute, AttributeValue singleValue)
            : this(attribute)
        {
            this.InternalValues.Add(singleValue);
        }

        /// <summary>
        /// Gets the schema attribute that represents the values in this collection
        /// </summary>
        public AcmaSchemaAttribute Attribute
        {
            get
            {
                return this.attribute;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this collection contains no values
        /// </summary>
        public virtual bool IsEmptyOrNull
        {
            get
            {
                return this.InternalValues.Count == 0;
            }
        }

        public int Count
        {
            get
            {
                return this.InternalValues.Count;
            }
        }

        /// <summary>
        /// Gets a list of AttributeValue objects stored in this collection
        /// </summary>
        public virtual IList<AttributeValue> Values
        {
            get
            {
                return this.InternalValues.AsReadOnly();
            }
        }

        public abstract void ClearValues();

        public abstract void ApplyValueChanges(IList<ValueChange> valueChanges);

        /// <summary>
        /// Gets or sets the underlying attribute values
        /// </summary>
        protected List<AttributeValue> InternalValues { get; set; }

        /// <summary>
        /// Compares the values in an AttributeValues with another object collection
        /// </summary>
        /// <param name="x">The AttributeValues to compare</param>
        /// <param name="y">The object to compare</param>
        /// <returns>A value indicating whether the two value sets are the same</returns>
        public static bool operator ==(AttributeValues x, object y)
        {
            return x.SequenceEquals(y);
        }

        /// <summary>
        /// Compares the values in an AttributeValues with another object collection
        /// </summary>
        /// <param name="x">The AttributeValues to compare</param>
        /// <param name="y">The object to compare</param>
        /// <returns>A value indicating whether the two value sets are not the same</returns>
        public static bool operator !=(AttributeValues x, object y)
        {
            return !x.SequenceEquals(y);
        }

        /// <summary>
        /// Compares the values in an AttributeValues with another object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>A value indicating whether the two objects contain the same values</returns>
        public override bool Equals(object obj)
        {
            return this.SequenceEquals(obj);
        }

        /// <summary>
        /// Compares the values in an AttributeValues with another object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>A value indicating whether the two objects contain the same values</returns>
        public bool SequenceEquals(object obj)
        {
            if (obj == null && this.InternalValues.Count == 0)
            {
                return true;
            }
            else if (obj == null)
            {
                return false;
            }

            if (obj is AttributeValues)
            {
                return this.InternalValues.SequenceEqual(((AttributeValues)obj).Values);
            }
            else if (obj is DBAttributeValues)
            {
                return this.InternalValues.SequenceEqual(((DBAttributeValues)obj).Values);
            }
            else if (obj is IList<object>)
            {
                return this.InternalValues.SequenceEqual((IList<object>)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Gets a value indicating if any of the values in this collection are equal to the value supplied
        /// </summary>
        /// <param name="obj">The value to test</param>
        /// <returns>A boolean value indicating if the object exists in the collection</returns>
        public virtual bool HasValue(object obj)
        {
            return this.InternalValues.Any(t => t.ValueEquals(obj));
        }

        /// <summary>
        /// Serves as a hash function for a particular type
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.InternalValues.Select(t => t.ToString()).ToCommaSeparatedString();
        }

        public IList<object> ToObjectList()
        {
            if (this.IsEmptyOrNull)
            {
                return new List<object>();
            }
            else
            {
                return this.Values.Select(t => t.Value).ToList();
            }
        }

        /// <summary>
        /// Gets a value indicating if two collections contain the same elements, regardless of their order
        /// </summary>
        /// <param name="list">The collection to compare against the values in this collection</param>
        /// <returns>True if the same values are found in both collections</returns>
        public bool ContainsAllElements(IEnumerable<object> list)
        {
            if (list.Count() != this.InternalValues.Count)
            {
                return false;
            }

            if (!this.Attribute.IsInAVPTable)
            {
                if (this.InternalValues.FirstOrDefault().Value == null)
                {
                    return false;
                }
            }

            switch (this.attribute.Type)
            {
                case ExtendedAttributeType.Binary:
                    IEnumerable<Binary> convertedList1 = this.InternalValues.Select(t => new Binary(t.ValueByte));
                    IEnumerable<Binary> convertedList2 = list.Select(t => new Binary(t as byte[]));
                    return convertedList1.Intersect(convertedList2).Count() == convertedList1.Count();

                case ExtendedAttributeType.Integer:
                    return this.InternalValues.Select(t => t.ValueLong).Intersect(list.Cast<long>()).Count() == this.InternalValues.Count;

                case ExtendedAttributeType.DateTime:
                    return this.InternalValues.Select(t => t.ValueDateTime).Intersect(list.Cast<DateTime>()).Count() == this.InternalValues.Count;

                case ExtendedAttributeType.Reference:
                    return this.InternalValues.Select(t => t.ValueGuid).Intersect(list.Cast<Guid>()).Count() == this.InternalValues.Count;

                case ExtendedAttributeType.String:
                    return this.InternalValues.Select(t => t.ValueString).Intersect(list.Cast<string>()).Count() == this.InternalValues.Count;

                case ExtendedAttributeType.Boolean:
                    return this.InternalValues.Select(t => t.ValueBoolean).Intersect(list.Cast<bool>()).Count() == this.InternalValues.Count;

                case ExtendedAttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.InternalValues.GetEnumerator();
        }

        IEnumerator<AttributeValue> IEnumerable<AttributeValue>.GetEnumerator()
        {
            return this.InternalValues.GetEnumerator();
        }

        /// <summary>
        /// Gets the values of the attribute converted to strings for serialization
        /// </summary>
        /// <returns></returns>
        internal IList<string> GetSerializationValues()
        {
            List<string> values = new List<string>();


            foreach (object value in this.Values.Select(t => t.Value))
            {
                values.Add(value.ToSmartString());
            }

            return values;
        }
    }
}
