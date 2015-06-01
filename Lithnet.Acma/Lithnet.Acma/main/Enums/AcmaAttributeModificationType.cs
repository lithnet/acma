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
    /// Defines the types of modifications allowed on an attribute
    /// </summary>
    [DataContract(Name = "acma-modification-type", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public enum AcmaAttributeModificationType
    {
        [Description("Add")]
        [EnumMember(Value = "add")]
        Add = 1,

        [Description("Replace")]
        [EnumMember(Value = "replace")]
        Replace = 2,

        [Description("Delete")]
        [EnumMember(Value = "delete")]
        Delete = 4,

        [Description("Conditional")]
        [EnumMember(Value = "conditional")]
        Conditional = 100
    }
}
