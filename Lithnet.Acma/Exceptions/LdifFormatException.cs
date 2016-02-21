// -----------------------------------------------------------------------
// <copyright file="LdifFormatException.cs" company="Lithnet">
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
    /// An exception that occurs when an error in an LDIF file is found
    /// </summary>
    [Serializable]
    public class LdifFormatException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of the LdifFormatException class
        /// </summary>
        public LdifFormatException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LdifFormatException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public LdifFormatException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the LdifFormatException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public LdifFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
