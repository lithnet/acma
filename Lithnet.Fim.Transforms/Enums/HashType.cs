// -----------------------------------------------------------------------
// <copyright file="HashType.cs" company="Lithnet">
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
    [DataContract(Name = "hash-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum HashType
    {
        /// <summary>
        /// SHA1 hash algorithm
        /// </summary>
        [Description("SHA1")]
        [EnumMember(Value = "sha1")]
        Sha1,

        /// <summary>
        /// SHA256 hash algorithm
        /// </summary>
        [Description("SHA256")]
        [EnumMember(Value = "sha256")]
        Sha256,

        /// <summary>
        /// SHA256 hash algorithm
        /// </summary>
        [Description("SHA384")]
        [EnumMember(Value = "sha384")]
        Sha384,

        /// <summary>
        /// SHA512 hash algorithm
        /// </summary>
        [Description("SHA512")]
        [EnumMember(Value = "sha512")]
        Sha512,
    }
}
