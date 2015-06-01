// -----------------------------------------------------------------------
// <copyright file="SchemaValidationException.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Thrown when an inconsistency is detected in the MA schema
    /// </summary>
    [Serializable]
    public class SchemaValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SchemaValidationException class
        /// </summary>
        public SchemaValidationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchemaValidationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SchemaValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchemaValidationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public SchemaValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
