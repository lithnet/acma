// -----------------------------------------------------------------------
// <copyright file="CircularUpdateException.cs" company="Lithnet">
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
    /// Thrown when a circular update is detected during a CSEntryChange commit 
    /// </summary>
    [Serializable]
    public class ConstructorExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConstructorExecutionException class
        /// </summary>
        public ConstructorExecutionException(string constructorID, Guid objectID)
            : base(string.Format("The constructor {0} encountered an exception when processing object {1}", constructorID, objectID))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConstructorExecutionException class
        /// </summary>
        public ConstructorExecutionException(string constructorID, Guid objectID, Exception innerException)
            : base(string.Format("The constructor {0} encountered an exception when processing object {1}", constructorID, objectID), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConstructorExecutionException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ConstructorExecutionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConstructorExecutionException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ConstructorExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
