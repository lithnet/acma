// -----------------------------------------------------------------------
// <copyright file="ReadOnlyValueException.cs" company="Lithnet">
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
    /// Thrown when an attempt is made to modify a read-only value
    /// </summary>
    [Serializable]
    public class DBVersionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DBVersionException class
        /// </summary>
        public DBVersionException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DBVersionException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public DBVersionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DBVersionException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public DBVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
