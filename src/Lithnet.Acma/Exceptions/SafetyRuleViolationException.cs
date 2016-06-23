// -----------------------------------------------------------------------
// <copyright file="ImportSafetyRuleViolationException.cs" company="Lithnet">
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
    /// Thrown when an attempt is made to modify a read-only value
    /// </summary>
    [Serializable]
    public class SafetyRuleViolationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ImportSafetyRuleViolationException class
        /// </summary>
        public SafetyRuleViolationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportSafetyRuleViolationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SafetyRuleViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportSafetyRuleViolationException class
        /// </summary>
        /// <param name="ruleName">The rule that as violated</param>
        /// <param name="value">The value that violated the rule</param>
        public SafetyRuleViolationException(string ruleName, string value)
            : base(string.Format("An import safety rule violation occurred for rule '{0}' by value '{1}'", ruleName, value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportSafetyRuleViolationException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public SafetyRuleViolationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
