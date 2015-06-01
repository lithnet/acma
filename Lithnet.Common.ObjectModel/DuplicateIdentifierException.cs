using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Common.ObjectModel
{
    public class DuplicateIdentifierException : Exception
    {
        public DuplicateIdentifierException()
            : base()
        {
        }

        public DuplicateIdentifierException(string message)
            : base(message)
        {
        }
    }
}
