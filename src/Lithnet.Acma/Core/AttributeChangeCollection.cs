using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Common.ObjectModel;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma
{
    public class AttributeChangeCollection : ObservableKeyedCollection<string, AttributeChange>
    {
        public AttributeChangeCollection()
            :base()
        {
        }

        protected override string GetKeyForItem(AttributeChange item)
        {
            return item.Name;
        }
    }
}
