// -----------------------------------------------------------------------
// <copyright file="AttributeValueDeleteConstructor.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A constructor used to delete an attribute's values
    /// </summary>
    [DataContract(Name = "value-delete-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Value delete")]
    public class AttributeValueDeleteConstructor : AttributeConstructor
    {
        /// <summary>
        /// Initializes a new instance of the AttributeValueDeleteConstructor class
        /// </summary>
        public AttributeValueDeleteConstructor()
            : base()
        {
        }

        protected override IEnumerable<SchemaAttributeUsage> GetAttributeUsageInternal(string parentPath, AcmaSchemaAttribute attribute)
        {
            yield break;
        }

        /// <summary>
        /// Executes the constructor
        /// </summary>
        /// <param name="hologram">The source object</param>
        internal override void Execute(MAObjectHologram hologram)
        {
            hologram.DeleteAttribute(this.Attribute);
            this.RaiseCompletedEvent();
        }
    }
}
