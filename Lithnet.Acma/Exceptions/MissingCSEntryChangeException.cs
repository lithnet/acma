// -----------------------------------------------------------------------
// <copyright file="MissingCSEntryChangeException.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Thrown when an attempt to modify an object that has not yet has a CSEntryChange applied to it
    /// </summary>
    [Serializable]
    public class ModificationTypeNotSetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MissingCSEntryChangeException class
        /// </summary>
        public ModificationTypeNotSetException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MissingCSEntryChangeException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ModificationTypeNotSetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MissingCSEntryChangeException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ModificationTypeNotSetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
