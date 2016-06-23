using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.Acma.TestEngine
{
    public class UnitTestEngineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnitTestEngineException class
        /// </summary>
        public UnitTestEngineException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnitTestEngineException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public UnitTestEngineException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnitTestEngineException class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public UnitTestEngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
