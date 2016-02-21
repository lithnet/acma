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
    public class DeclarationExpansionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DeclarationExpansionException class
        /// </summary>
        public DeclarationExpansionException(string declaration, string objectID)
            : base(string.Format("An exception was encountered trying to expand the declaration for object ID {1}. Declaration string: {0}", declaration, objectID))
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeclarationExpansionException class
        /// </summary>
        public DeclarationExpansionException(string declaration, string objectID, Exception innerException)
            : base(string.Format("An exception was encountered trying to expand the declaration for object ID {1}. Declaration string: {0}", declaration, objectID), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeclarationExpansionException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public DeclarationExpansionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DeclarationExpansionException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public DeclarationExpansionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
