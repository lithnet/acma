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
    [DataContract(Name = "string-case-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum StringCaseType
    {
        /// <summary>
        /// Do not change the case of the string
        /// </summary>
        [Description("Do not change the case")]
        [EnumMember(Value = "none")]
        Default = 0,

        /// <summary>
        /// Change the string to lower case
        /// </summary>
        [Description("Change to lower case")]
        [EnumMember(Value = "lower")]
        Lower = 1,

        /// <summary>
        /// Change the string to upper case
        /// </summary>
        [Description("Change to upper case")]
        [EnumMember(Value = "upper")]
        Upper = 2,

        /// <summary>
        /// Change the string to title case
        /// </summary>
        [Description("Change to title case")]
        [EnumMember(Value = "title")]
        Title = 3
    }
}
