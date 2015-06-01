// -----------------------------------------------------------------------
// <copyright file="UnknownOrUnsupportedModificationTypeException.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// An exception that occurs when an unknown or unsupported modification type is used
    /// </summary>
    [Serializable]
    public class UnknownOrUnsupportedModificationTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedModificationTypeException class
        /// </summary>
        public UnknownOrUnsupportedModificationTypeException()
            : base("An unknown or unsupported modification type was used")
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedModificationTypeException class
        /// </summary>
        /// <param name="modificationType">The modification type that was unknown or supported</param>
        public UnknownOrUnsupportedModificationTypeException(ObjectModificationType modificationType)
            : base(string.Format("An unknown or unsupported object modification type was used: {0}", modificationType.ToSmartString()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedModificationTypeException class
        /// </summary>
        /// <param name="modificationType">The modification type that was unknown or supported</param>
        public UnknownOrUnsupportedModificationTypeException(ValueModificationType modificationType)
            : base(string.Format("An unknown or unsupported value modification type was used: {0}", modificationType.ToSmartString()))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedModificationTypeException class
        /// </summary>
        /// <param name="modificationType">The modification type that was unknown or supported</param>
        public UnknownOrUnsupportedModificationTypeException(AttributeModificationType modificationType)
            : base(string.Format("An unknown or unsupported attribute modification type was used: {0}", modificationType.ToSmartString()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedModificationTypeException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public UnknownOrUnsupportedModificationTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedModificationTypeException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public UnknownOrUnsupportedModificationTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
