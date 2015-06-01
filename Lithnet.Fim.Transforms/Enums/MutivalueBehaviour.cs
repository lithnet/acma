using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [Flags]
    [DataContract(Name = "mv-behavior", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum MutivalueBehaviour
    {
        [Description("Multivalued attributes are not supported")]
        [EnumMember(Value = "not-supported")]
        NotSupported = 0,

        [Description("Multivalued attribute values are processed individually")]
        [EnumMember(Value = "individual")]
        Individual = 1,

        [Description("Multivalued attribute values are processed together")]
        [EnumMember(Value = "grouped")]
        Grouped = 2
    }
}
