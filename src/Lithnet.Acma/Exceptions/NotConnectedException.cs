// -----------------------------------------------------------------------
// <copyright file="NotConnectedException.cs" company="Ryan Newington">
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
    /// Thrown when not active connection exists
    /// </summary>
    [Serializable]
    public class NotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NotConnectedException class
        /// </summary>
        public NotConnectedException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotConnectedException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public NotConnectedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotConnectedException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public NotConnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
