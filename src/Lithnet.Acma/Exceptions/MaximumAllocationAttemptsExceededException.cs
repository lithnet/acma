// -----------------------------------------------------------------------
// <copyright file="MaximumAllocationAttemptsExceededException.cs" company="Lithnet">
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
    /// An exception that occurs when the maximum number of unique allocation attempts has been exceeded
    /// </summary>
    [Serializable]
    public class MaximumAllocationAttemptsExceededException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MaximumAllocationAttemptsExceededException class
        /// </summary>
        public MaximumAllocationAttemptsExceededException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaximumAllocationAttemptsExceededException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public MaximumAllocationAttemptsExceededException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the MaximumAllocationAttemptsExceededException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public MaximumAllocationAttemptsExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
