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
    using System.ComponentModel;

    /// <summary>
    /// The rule to apply to the execution of items within the group
    /// </summary>
    [DataContract(Name = "group-execution-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public enum GroupExecutionRule
    {
        /// <summary>
        /// Executes all objects within the group
        /// </summary>
        [EnumMember(Value = "execute-all")]
        [Description("Execute all objects")]
        ExecuteAll,

        /// <summary>
        /// Exits the group when the first constructor that meets the execution rules completes
        /// </summary>
        [EnumMember(Value = "exit-after-first-success")]
        [Description("Exit after first execution")]
        ExitAfterFirstSuccess
    }
}
