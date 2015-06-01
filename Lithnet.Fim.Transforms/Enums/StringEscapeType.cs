// -----------------------------------------------------------------------
// <copyright file="StringCase.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// The type of casing to apply to a string
    /// </summary>
    [DataContract(Name = "string-escape-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum StringEscapeType
    {
        /// <summary>
        /// Escapes XML content
        /// </summary>
        [Description("XML content")]
        [EnumMember(Value = "none")]
        Xml = 0,

        /// <summary>
        /// Escape DN components
        /// </summary>
        [Description("LDAP DN component")]
        [EnumMember(Value = "lower")]
        LdapDN = 1,
    }
}
