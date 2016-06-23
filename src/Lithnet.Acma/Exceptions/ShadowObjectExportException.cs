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
    /// Thrown when an invalid attempt is made to export a shadow object
    /// </summary>
    [Serializable]
    public class ShadowObjectExportException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DuplicateObjectException class
        /// </summary>
        public ShadowObjectExportException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DuplicateObjectException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ShadowObjectExportException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DuplicateObjectException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ShadowObjectExportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
