// -----------------------------------------------------------------------
// <copyright file="MissingValueException.cs" company="Lithnet">
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
    /// Thrown when a value is expected but is not present
    /// </summary>
    [Serializable]
    public class MissingValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MissingValueException class
        /// </summary>
        public MissingValueException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MissingValueException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public MissingValueException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MissingValueException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public MissingValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
