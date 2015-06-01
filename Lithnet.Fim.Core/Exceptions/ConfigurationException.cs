// -----------------------------------------------------------------------
// <copyright file="ConfigurationException.cs" company="Lithnet">
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
    /// An exception that is thrown when a misconfiguration is detected in the configuration file
    /// </summary>
    [Serializable]
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConfigurationException class
        /// </summary>
        public ConfigurationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConfigurationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ConfigurationException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the ConfigurationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
