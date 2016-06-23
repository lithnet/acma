using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lithnet.Acma
{
    public enum HologramView
    {
        [Description("Proposed")]
        Proposed = 0,

        [Description("Existing")]
        Current
    }
}
