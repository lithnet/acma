// -----------------------------------------------------------------------
// <copyright file="UnknownOrUnsupportedValueOperatorException.cs" company="Lithnet">
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
    /// Thrown when the value operator used to compare a specified data type is unsupported
    /// </summary>
    [Serializable]
    public class UnknownOrUnsupportedValueOperatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedValueOperatorException class
        /// </summary>
        public UnknownOrUnsupportedValueOperatorException()
            : base("The specified value operator is unknown or unsupported")
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedValueOperatorException class
        /// </summary>
        /// <param name="valueOperator">The value operator that was unknown or unsupported</param>
        /// <param name="t">The type that the value operator cannot support</param>
        public UnknownOrUnsupportedValueOperatorException(ValueOperator valueOperator, Type t)
            : base(string.Format("The value operator '{0}' cannot be used on a {1} type", valueOperator.ToString(), t.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedValueOperatorException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public UnknownOrUnsupportedValueOperatorException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the UnknownOrUnsupportedValueOperatorException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public UnknownOrUnsupportedValueOperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
