// -----------------------------------------------------------------------
// <copyright file="TrimType.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies a type of trimming to apply to a string
    /// </summary>
    [DataContract(Name = "trim-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum TrimType
    {
        /// <summary>
        /// Do not trim the string
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Trim the left side of the string
        /// </summary>
        [EnumMember(Value = "left")]
        Left,

        /// <summary>
        /// Trim the right side of the string
        /// </summary>
        [EnumMember(Value = "right")]
        Right,

        /// <summary>
        /// Trim both ends of the string
        /// </summary>
        [EnumMember(Value = "both")]
        Both
    }
}
