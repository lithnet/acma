using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Common.ObjectModel
{
    public class ValidationException : Exception
    {
        public ValidationException(string propertyName, string errorMessage)
            : base(string.Format("A data validation occurred on property {0}. {1}", propertyName, errorMessage))
        {
        }

        public ValidationException()
            : base()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
