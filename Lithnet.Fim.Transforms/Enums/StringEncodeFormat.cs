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
    /// Specifies the format to use when converting a value to a string
    /// </summary>
    [DataContract(Name = "string-encode-format", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum StringEncodeFormat
    {
        /// <summary>
        /// Base-64
        /// </summary>
        [Description("Base-64")]
        [EnumMember(Value = "base64")]
        Base64,

        /// <summary>
        /// Base-32 algorithm
        /// </summary>
        [Description("Base-32")]
        [EnumMember(Value = "base32")]
        Base32,
    }
}
