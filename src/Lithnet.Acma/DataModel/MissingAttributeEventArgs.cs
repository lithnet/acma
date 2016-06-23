using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma.DataModel
{
    public class MissingAttributeEventArgs
    {
        public MissingAttributeEventArgs(string missingAttribute)
        {
            this.MissingAttributeName = missingAttribute;
        }

        public AcmaSchemaAttribute ReplacementAttribute { get; set; }

        public string MissingAttributeName { get; private set; }
    }
}
