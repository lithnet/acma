// -----------------------------------------------------------------------
// <copyright file="QueryValueNullException.cs" company="Lithnet">
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
    /// An exception that is thrown when a query cannot be constructed due to a missing value
    /// </summary>
    [Serializable]
    public class QueryValueNullException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the QueryValueNullException class
        /// </summary>
        public QueryValueNullException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the QueryValueNullException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public QueryValueNullException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the QueryValueNullException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public QueryValueNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
