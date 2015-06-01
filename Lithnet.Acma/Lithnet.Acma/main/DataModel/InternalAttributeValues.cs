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
    using Lithnet.Fim.Core;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Represents a collection of AttributeValue objects
    /// </summary>
    public class InternalAttributeValues : AttributeValues
    {
        /// <summary>
        /// Initializes a new instance of the AttributeValues class
        /// </summary>
        /// <param name="attribute">The schema attribute representing this value collection</param>
        public InternalAttributeValues(AcmaSchemaAttribute attribute)
            : base(attribute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValues class
        /// </summary>
        /// <param name="attribute">The schema attribute representing this value collection</param>
        /// <param name="values">The list of values to assign to this object</param>
        public InternalAttributeValues(AcmaSchemaAttribute attribute, IList<object> values)
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

        public override void ClearValues()
        {
            this.InternalValues.Clear();
        }

        public override void ApplyValueChanges(IList<ValueChange> valueChanges)
        {
            throw new NotImplementedException();
        }
    }
}
