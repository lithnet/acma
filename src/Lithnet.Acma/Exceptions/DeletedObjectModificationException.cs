// -----------------------------------------------------------------------
// <copyright file="DeletedObjectModificationException.cs" company="Ryan Newington">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Thrown when an attempt is made to modify an object that is marked for deletion
    /// </summary>
    [Serializable]
    public class DeletedObjectModificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DeletedObjectModificationException class
        /// </summary>
        public DeletedObjectModificationException()
            : base("An attempt was made to modify an object that is marked for deletion")
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeletedObjectModificationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public DeletedObjectModificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeletedObjectModificationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public DeletedObjectModificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
