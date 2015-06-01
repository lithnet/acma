// -----------------------------------------------------------------------
// <copyright file="ValueOperator.cs" company="Ryan Newington">
// Copyright (c)
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// A list of possible comparison operators
    /// </summary>
    public enum ValueOperator
    {
        /// <summary>
        /// No comparison type is specified
        /// </summary>
        None,

        /// <summary>
        /// The values being compared must be the same
        /// </summary>
        [Description("Equals")]
        Equals,

        /// <summary>
        /// The values being compared must not be the same 
        /// </summary>
        [Description("Does not equal")]
        NotEquals,
        
        /// <summary>
        /// The value being compared must be greater than the value it is being compared to
        /// </summary>
        [Description("Is greater than")]
        GreaterThan,

        /// <summary>
        /// The value being compared must be less than the value it is being compared to
        /// </summary>
        [Description("Is less than")]
        LessThan,

        /// <summary>
        /// The value being compared must be greater than or equal to the value it is being compared to
        /// </summary>
        [Description("Is greater than or equal to")]
        GreaterThanOrEq,

        /// <summary>
        /// The value being compared must be less than or equal to the value it is being compared to
        /// </summary>
        [Description("Is less than or equal to")]
        LessThanOrEq,

        /// <summary>
        /// The value being compared must be found in the value it is being compared to
        /// </summary>
        [Description("Contains")]
        Contains,

        /// <summary>
        /// The value being compared must not be found in the value it is being compared to
        /// </summary>
        [Description("Does not contain")]
        NotContains,

        /// <summary>
        /// The value being compared must start with the value it is being compared to
        /// </summary>
        [Description("Starts with")]
        StartsWith,

        /// <summary>
        /// The value being compared must end with the value it is being compared to
        /// </summary>
        [Description("Ends with")]
        EndsWith,

        /// <summary>
        /// The values are compared with a logical AND
        /// </summary>
        [Description("And")]
        And,

        /// <summary>
        /// The values are compared with a logical OR
        /// </summary>
        [Description("Or")]
        Or,

        /// <summary>
        /// The selector should take the smallest value found
        /// </summary>
        [Description("Smallest")]
        Smallest,

        /// <summary>
        /// The selector should take the largest value found
        /// </summary>
        [Description("Largest")]
        Largest,

        /// <summary>
        /// The value must not be null, empty, or missing
        /// </summary>
        [Description("Is present")]
        IsPresent,

        /// <summary>
        /// The value must be null, empty, or missing
        /// </summary>
        [Description("Is not present")]
        NotPresent,

        /// <summary>
        /// Gets the first value in a set of values
        /// </summary>
        [Description("First")]
        First,

        /// <summary>
        /// Gets the last value from a set of values
        /// </summary>
        [Description("Last")]
        Last
    }
}
