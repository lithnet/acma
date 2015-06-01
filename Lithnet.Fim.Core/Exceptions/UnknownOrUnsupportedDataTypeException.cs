// -----------------------------------------------------------------------
// <copyright file="UnknownOrUnsupportedDataTypeException.cs" company="Lithnet">
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
    /// Thrown when a data type is specified that is unknown or unsupported in the current context
    /// </summary>
    [Serializable]
    public class UnknownOrUnsupportedDataTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedDataTypeException class
        /// </summary>
        public UnknownOrUnsupportedDataTypeException()
            : base("The specified data type is unknown or unsupported")
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedDataTypeException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public UnknownOrUnsupportedDataTypeException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedDataTypeException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public UnknownOrUnsupportedDataTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
