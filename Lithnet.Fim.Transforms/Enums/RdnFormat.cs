// -----------------------------------------------------------------------
// <copyright file="RdnFormat.cs" company="Lithnet">
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
    /// Specifies the format to use when parsing a relative distinguished name component
    /// </summary>
    [DataContract(Name = "rdn-format", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum RdnFormat
    {
        /// <summary>
        /// Only use the value component of the RDN
        /// </summary>
        /// <example>An RDN of 'CN=user1' will return 'user1'</example>
        [Description("Use the value only")]
        [EnumMember(Value = "value")]
        ValueOnly,

        /// <summary>
        /// Use the full RDN
        /// </summary>
        /// <example>An RDN of 'CN=user1' will return 'CN=user1'</example>
        [Description("Use the full RDN")]
        [EnumMember(Value = "rdn")]
        Rdn
    }
}
