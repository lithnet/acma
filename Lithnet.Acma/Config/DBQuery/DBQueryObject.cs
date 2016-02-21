// -----------------------------------------------------------------------
// <copyright file="DBQueryObject.cs" company="Lithnet">
// Copyright (c) 2014
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Runtime.Serialization;
    using Lithnet.Common.ObjectModel;
    using Lithnet.Acma.DataModel;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an abstract base class for dynamic database query objects
    /// </summary>
    [KnownType(typeof(DBQuery))]
    [KnownType(typeof(DBQueryGroup))]
    [DataContract(Name = "dbquery", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public abstract class DBQueryObject : UINotifyPropertyChanges, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the description of the object
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        public abstract IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute);
        
        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
