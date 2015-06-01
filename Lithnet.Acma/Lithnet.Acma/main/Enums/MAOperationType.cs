// -----------------------------------------------------------------------
// <copyright file="MAOperationType.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The type of operation in progress on the MA
    /// </summary>
    public enum MAOperationType
    {
        /// <summary>
        /// No operation
        /// </summary>
        None = 0,

        /// <summary>
        /// An import operation
        /// </summary>
        Import = 1,

        /// <summary>
        /// An export operation
        /// </summary>
        Export = 2,

        /// <summary>
        /// A password change or set operation
        /// </summary>
        Password = 3
    }
}
