using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Lithnet.Acma
{
    [DataContract(Name = "event-operation-type", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Flags]
    public enum AcmaEventOperationType
    {
        [EnumMember(Value = "full-import")]
        [Description("Full import")]
        FullImport = 1,
        
        [EnumMember(Value = "delta-import")]
        [Description("Delta import")]
        DeltaImport = 2,
        
        [EnumMember(Value = "export")]
        [Description("Export")]
        Export = 4
    }
}
