using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace Lithnet.Fim.Transforms
{
    public class LookupItemComparer : IComparer, IComparer<LookupItem>
    {
        public int Compare(object x, object y)
        {
            if (x == y)
            {
                return 0;
            }
            else if (x is LookupItem && y is LookupItem)
            {
                return this.Compare(x as LookupItem, y as LookupItem);
            }
            else
            {
                return 1;
            }
        }

        public int Compare(LookupItem x, LookupItem y)
        {
            if (x == null || y == null)
            {
                return 1;
            }

            int retval = string.Compare(x.CurrentValue, y.CurrentValue);

            if (retval == 0)
            {
                return string.Compare(x.NewValue, y.NewValue);
            }

            return retval;
        }
    }
}
