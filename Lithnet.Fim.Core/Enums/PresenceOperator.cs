// -----------------------------------------------------------------------
// <copyright file="PresenceOperator.cs" company="Ryan Newington">
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
    /// A list of possible presence comparison operators
    /// </summary>
    public enum PresenceOperator
    {
        /// <summary>
        /// The value must not be null, empty, or missing
        /// </summary>
        [Description("Is present")]
        IsPresent,

        /// <summary>
        /// The value must be null, empty, or missing
        /// </summary>
        [Description("Is not present")]
        NotPresent
    }
}
