// -----------------------------------------------------------------------
// <copyright file="InheritedValueModificationException.cs" company="Lithnet">
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
    /// An exception that occurs when an attempt is made to modify an inherited value 
    /// </summary>
    [Serializable]
    public class InheritedValueModificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InheritedValueModificationException class
        /// </summary>
        public InheritedValueModificationException()
            : base("An attempt was made to modify a value inherited from another object")
        {
        }

        /// <summary>
        /// Initializes a new instance of the InheritedValueModificationException class
        /// </summary>
        /// <param name="attributeName">The name of the inherited attribute</param>
        public InheritedValueModificationException(string attributeName)
            : base(string.Format("An attempt was made to modify a value inherited from another object: {0}", attributeName))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the InheritedValueModificationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public InheritedValueModificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
