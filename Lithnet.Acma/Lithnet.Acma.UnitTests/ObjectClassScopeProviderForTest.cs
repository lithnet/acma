using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    public class ObjectClassScopeProviderForTest: IObjectClassScopeProvider
    {
        public ObjectClassScopeProviderForTest(string name)
        {
            this.ObjectClass = ActiveConfig.DB.GetObjectClass(name);
        }

        public AcmaSchemaObjectClass ObjectClass { get; set; }
    }
}
