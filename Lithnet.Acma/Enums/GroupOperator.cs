// -----------------------------------------------------------------------
// <copyright file="GroupOperator.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// The type of comparison to make between objects in a group to determine if the group result is successful
    /// </summary>
    [DataContract(Name = "group-operator", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public enum GroupOperator
    {
        /// <summary>
        /// No rules are allowed to pass for the group to pass
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// All rules in the group must pass for the group to pass
        /// </summary>
        [EnumMember(Value = "all")]
        All,

        /// <summary>
        /// At least one rule in the group must pass in order for the group to pass
        /// </summary>
        [EnumMember(Value = "any")]
        Any,

        /// <summary>
        /// Exactly one rule in the group must pass for the group to pass
        /// </summary>
        [EnumMember(Value = "one")]
        One
    }
}
