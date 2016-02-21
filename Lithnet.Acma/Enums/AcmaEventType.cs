using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Acma
{
    [DataContract(Name = "acma-event-type", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Flags]
    public enum AcmaEventType
    {
        [EnumMember(Value = "internal-exit-event")]
        InternalExitEvent,

        [EnumMember(Value = "external-exit-event")]
        ExternalExitEvent,

        [EnumMember(Value = "operation-event")]
        OperationEvent
    }
}
