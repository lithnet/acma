using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma
{
    public interface IAttributeScopeProvider
    {
        AcmaSchemaAttribute Attribute { get; }
    }
}
