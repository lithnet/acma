// -----------------------------------------------------------------------
// <copyright file="ValueChangeComparer.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Compares two value changes ensuring the contain the same value and modification type
    /// </summary>
    public class ValueChangeComparer : IEqualityComparer<ValueChange>
    {
        /// <summary>
        /// The schema attribute that this comparer compares
        /// </summary>
        private AcmaSchemaAttribute attribute;

        /// <summary>
        /// A value indicating whether the modification type should be considered when evaluating if two ValueChange objects are equal
        /// </summary>
        private bool compareModificationType;

        /// <summary>
        /// Initializes a new instance of the ValueChangeComparer class
        /// </summary>
        /// <param name="attribute">The schema attribute that this comparer compares</param>
        /// <param name="includeModificationTypeInComparison">A value indicating if the modification type should be taken into account when comparing ValueChanges for equality. If this is set to false, then only the values are compared</param>
        public ValueChangeComparer(AcmaSchemaAttribute attribute, bool includeModificationTypeInComparison)
        {
            this.attribute = attribute;
            this.compareModificationType = includeModificationTypeInComparison;
        }

        /// <summary>
        /// Compares two ValueChange objects
        /// </summary>
        /// <param name="x">The first value to compare</param>
        /// <param name="y">The second value to compare</param>
        /// <returns>A value indicating whether the two objects are the same</returns>
        public bool Equals(ValueChange x, ValueChange y)
        {
            if (x == null ^ y == null)
            {
                return false;
            }

            if (x == null && y == null)
            {
                return true;
            }

            if (this.compareModificationType)
            {
                if (x.ModificationType != y.ModificationType)
                {
                    return false;
                }
            }
             
            return new AttributeValue(this.attribute, x.Value) == new AttributeValue(this.attribute, y.Value);
        }

        /// <summary>
        /// Gets the hash code for the specified object
        /// </summary>
        /// <param name="obj">The object to obtain the hash code for</param>
        /// <returns>The hash code of the specified object</returns>
        public int GetHashCode(ValueChange obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.Value == null ? 0 : obj.Value.GetHashCode();
            }
        }
    }
}
