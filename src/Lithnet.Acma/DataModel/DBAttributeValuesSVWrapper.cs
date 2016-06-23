// -----------------------------------------------------------------------
// <copyright file="DBAttributeValues.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Represents a collection of DBAttributeValue objects
    /// </summary>
    public class DBAttributeValuesSVWrapper : AttributeValues
    {
        private DBAttributeValue internalValue;

        public DBAttributeValuesSVWrapper(DBAttributeValue value)
            : base(value.Attribute)
        {
            this.internalValue = value;
            this.InternalValues.Add(value);
        }

        /// <summary>
        /// Gets a value indicating whether any values in the collection have changed
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return this.internalValue.HasChanged;
            }
        }

        /// <summary>
        /// Gets a read-only list of values contained in this collection
        /// </summary>
        public override IList<AttributeValue> Values
        {
            get
            {
                return this.InternalValues.AsReadOnly();
            }
        }

        public override bool IsEmptyOrNull
        {
            get
            {
                return this.internalValue.IsNull;
            }
        }

        /// <summary>
        /// Checks the collection of values for the presence of a particular value
        /// </summary>
        /// <param name="obj">The value to test for</param>
        /// <returns>A value indicating if the specified value is contained in the collection of values</returns>
        public override bool HasValue(object obj)
        {
            return this.internalValue.ValueEquals(obj);
        }

        /// <summary>
        /// Adds a new value to the collection
        /// </summary>
        /// <param name="obj">The value to add</param>
        public void AddValue(object obj)
        {
            this.internalValue.SetValue(obj);
        }

        /// <summary>
        /// Removes a value from the collection
        /// </summary>
        /// <param name="obj">The value to remove</param>
        public void RemoveValue(object obj)
        {
            if (this.internalValue.ValueEquals(obj))
            {
                this.internalValue.SetValue(null);
            }
        }

        /// <summary>
        /// Removes all values from the collection
        /// </summary>
        public override void ClearValues()
        {
            this.internalValue.SetValue(null);
        }

        /// <summary>
        /// Applies a set of ValueChange objects to the collection of values
        /// </summary>
        /// <param name="valueChanges">The list of value changes to make</param>
        public override void ApplyValueChanges(IList<ValueChange> valueChanges)
        {
            int adds = valueChanges.Count(t => t.ModificationType == ValueModificationType.Add);

            if (adds > 1)
            {
                throw new TooManyValuesException(this.Attribute.Name);
            }
            if (adds == 1)
            {
                this.AddValue(valueChanges.First(t => t.ModificationType == ValueModificationType.Add).Value);
            }
            else
            {
                foreach (ValueChange valueChange in valueChanges.Where(t => t.ModificationType == ValueModificationType.Delete))
                {
                    this.RemoveValue(valueChange.Value);
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.internalValue.Value.ToSmartStringOrNull();
        }
    }
}