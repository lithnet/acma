// -----------------------------------------------------------------------
// <copyright file="AcmaAttributeOperation.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the types of operations allowed on an attribute
    /// </summary>
    [DataContract(Name = "attribute-operation", Namespace="http://lithnet.local/Lithnet.Acma/v1/")]
    public enum AcmaAttributeOperation
    {
        /// <summary>
        /// The attribute can be imported from and exported to this MA
        /// </summary>
        [Description("Import/Export")]
        [EnumMember(Value = "import-export")]
        ImportExport = 0,

        /// <summary>
        /// The attribute can only be exported to this MA
        /// </summary>
        [Description("Export from FIM")]
        [EnumMember(Value = "export-only")]
        ExportOnly = 1,

        /// <summary>
        /// The attribute can only be imported from this MA
        /// </summary>
        [Description("Import to FIM")]
        [EnumMember(Value = "import-only")]
        ImportOnly = 2,

        /// <summary>
        /// The attribute cannot be imported or exported to this MA. It is not exposed to the FIM sync engine, and can only be used internally within ACMA
        /// </summary>
        [Description("Internal (not visible to FIM)")]
        [EnumMember(Value = "acma-internal")]
        AcmaInternal = 3,

        /// <summary>
        /// The attribute is used for temporary data storage and does not physically exist in the database. The value is discarded after a commit operation.
        /// </summary>
        [Description("Temporary (not visible to FIM or stored in DB)")]
        [EnumMember(Value = "acma-internal-temp")]
        AcmaInternalTemp = 4,
    }
}
