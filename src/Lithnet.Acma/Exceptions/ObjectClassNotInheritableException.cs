// -----------------------------------------------------------------------
// <copyright file="ObjectClassNotInheritableException.cs" company="Lithnet">
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
    /// Thrown when an attempt is made to specify inheritance on an object that has not inheritance mappings on it
    /// </summary>
    [Serializable]
    public class ObjectClassNotInheritableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ObjectClassNotInheritableException class
        /// </summary>
        public ObjectClassNotInheritableException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectClassNotInheritableException class
        /// </summary>
        /// <param name="objectClass">The name of the object class that cannot be inherited from</param>
        public ObjectClassNotInheritableException(string objectClass)
            : base(string.Format("The object class '{0}' cannot be inherited from as it contains no shadow object reference definitions", objectClass))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectClassNotInheritableException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ObjectClassNotInheritableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
