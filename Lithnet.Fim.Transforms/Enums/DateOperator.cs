// -----------------------------------------------------------------------
// <copyright file="DateOperator.cs" company="Lithnet">
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
    /// A date operation
    /// </summary>
    [DataContract(Name = "date-operator", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum DateOperator
    {
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Indicates a date addition
        /// </summary>
        [EnumMember(Value = "add")]
        Add,

        /// <summary>
        /// Indicates a date subtraction
        /// </summary>
        [EnumMember(Value = "subtract")]
        Subtract
    }
}
