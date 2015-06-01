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
    /// A number operation
    /// </summary>
    [DataContract(Name = "number-operator", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum NumberOperator
    {
        /// <summary>
        /// Indicates a number addition
        /// </summary>
        [Description("Add the transform value to the incoming value")]
        [EnumMember(Value = "add")]
        Add,

        /// <summary>
        /// Indicates a number subtraction
        /// </summary>
        [EnumMember(Value = "subtract-from-incoming")]
        [Description("Subtract the transform value from the incoming value")]
        SubtractFromIncoming,

        /// <summary>
        /// Indicates a number subtraction
        /// </summary>
        [EnumMember(Value = "subtract-from-target")]
        [Description("Subtract the incoming value from the transform value")]
        SubtractFromTarget,

        /// <summary>
        /// Indicates a number division
        /// </summary>
        [EnumMember(Value = "divide-by-incoming")]
        [Description("Divide the transform value by the incoming value")]
        DivideByIncoming,

        /// <summary>
        /// Indicates a number division
        /// </summary>
        [EnumMember(Value = "divide-by-target")]
        [Description("Divide the incoming value by the transform value")]
        DivideByTarget,

        /// <summary>
        /// Indicates a number multiplication
        /// </summary>
        [EnumMember(Value = "multiply")]
        [Description("Multiply the transform value by the incoming value")]
        Multiply
    }
}
