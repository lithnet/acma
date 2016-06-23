using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma.DataModel
{
    public partial class AcmaConstant
    {
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Name, this.Value);
        }
    }
}
