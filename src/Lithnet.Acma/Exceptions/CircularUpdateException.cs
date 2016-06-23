// -----------------------------------------------------------------------
// <copyright file="CircularUpdateException.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Thrown when a circular update is detected during a CSEntryChange commit 
    /// </summary>
    [Serializable]
    public class CircularUpdateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CircularUpdateException class
        /// </summary>
        public CircularUpdateException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CircularUpdateException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public CircularUpdateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CircularUpdateException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public CircularUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
