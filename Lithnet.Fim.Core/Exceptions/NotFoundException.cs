// -----------------------------------------------------------------------
// <copyright file="NotFoundException.cs" company="Ryan Newington">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Thrown when an expected object or value was not found
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NotFoundException class
        /// </summary>
        public NotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
