// -----------------------------------------------------------------------
// <copyright file="MultipleMatchException.cs" company="Lithnet">
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
    /// Thrown when more than one result to search was found, when only a single match is permitted
    /// </summary>
    [Serializable]
    public class MultipleMatchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MultipleMatchException class
        /// </summary>
        public MultipleMatchException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipleMatchException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public MultipleMatchException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipleMatchException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public MultipleMatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
