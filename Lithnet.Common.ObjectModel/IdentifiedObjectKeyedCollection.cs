using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Lithnet.Common.ObjectModel
{
    public class IdentifiedObjectKeyedCollection : KeyedCollection<string, IIdentifiedObject>
    {
        protected override string GetKeyForItem(IIdentifiedObject item)
        {
            return item.ID;
        }
    }
}
