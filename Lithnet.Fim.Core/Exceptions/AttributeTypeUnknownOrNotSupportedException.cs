// -----------------------------------------------------------------------
// <copyright file="AttributeTypeUnknownOrNotSupportedException.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class AttributeTypeUnknownOrNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AttributeTypeUnknownOrNotSupportedException class
        /// </summary>
        public AttributeTypeUnknownOrNotSupportedException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeTypeUnknownOrNotSupportedException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public AttributeTypeUnknownOrNotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeTypeUnknownOrNotSupportedException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public AttributeTypeUnknownOrNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
