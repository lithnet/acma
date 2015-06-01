// -----------------------------------------------------------------------
// <copyright file="PadType.cs" company="Lithnet">
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
    /// Defines the type of padding to apply to a substring operation
    /// </summary>
    [DataContract(Name = "pad-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum PadType
    {
        /// <summary>
        /// Apply no padding
        /// </summary>
        [Description("Apply no padding")]
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Use the first character of the string
        /// </summary>
        [Description("Repeat the first character")]
        [EnumMember(Value = "first-character")]
        FirstCharacter,

        /// <summary>
        /// Use the last character of the string
        /// </summary>
        [Description("Repeat the last character")]
        [EnumMember(Value = "last-character")]
        LastCharacter,

        /// <summary>
        /// Use the specified value
        /// </summary>
        [Description("Use a specific value")]
        [EnumMember(Value = "specific-value")]
        SpecifiedValue
    }
}
