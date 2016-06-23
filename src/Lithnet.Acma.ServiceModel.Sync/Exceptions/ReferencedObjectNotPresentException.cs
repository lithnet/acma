// -----------------------------------------------------------------------
// <copyright file="ReferencedObjectNotPresentException.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Thrown when one object that is referenced by another is not present 
    /// </summary>
    [Serializable]
    public class ReferencedObjectNotPresentException : Exception
    {
         /// <summary>
        /// Initializes a new instance of the ReferencedObjectNotPresentException class
        /// </summary>
        public ReferencedObjectNotPresentException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReferencedObjectNotPresentException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ReferencedObjectNotPresentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReferencedObjectNotPresentException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ReferencedObjectNotPresentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
