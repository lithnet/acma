// -----------------------------------------------------------------------
// <copyright file="OnMissingMatch.cs" company="Lithnet">
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
    /// Defines an action to take when a match is not found
    /// </summary>
    [DataContract(Name = "on-missing-match", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum OnMissingMatch
    {
        /// <summary>
        /// Return null
        /// </summary>
        [Description("Return null")]
        [EnumMember(Value = "return-null")]
        UseNull,

        /// <summary>
        /// Return the original value
        /// </summary>
        [Description("Return the original value")]
        [EnumMember(Value = "return-original")]
        UseOriginal,

        /// <summary>
        /// Use a specified default value
        /// </summary>
        [Description("Use a specified value")]
        [EnumMember(Value = "return-specified")]
        UseDefault
    }
}
