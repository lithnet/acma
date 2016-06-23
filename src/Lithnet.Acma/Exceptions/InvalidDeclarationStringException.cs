// -----------------------------------------------------------------------
// <copyright file="InvalidDeclarationStringException.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lithnet.MetadirectoryServices;
    
    /// <summary>
    /// An exception that occurs when a declaration string is invalid
    /// </summary>
    [Serializable]
    public class InvalidDeclarationStringException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidDeclarationStringException class
        /// </summary>
        public InvalidDeclarationStringException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidDeclarationStringException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public InvalidDeclarationStringException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the InvalidDeclarationStringException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public InvalidDeclarationStringException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
