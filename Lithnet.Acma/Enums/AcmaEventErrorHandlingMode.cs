using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Lithnet.Acma
{
    [DataContract(Name = "event-error-handling-mode", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Flags]
    public enum AcmaEventErrorHandlingMode
    {
        [EnumMember(Value = "log")]
        [Description("Log the error")]
        Log,

        [EnumMember(Value = "throw")]
        [Description("Abort the export")]
        Throw,
    }
}
