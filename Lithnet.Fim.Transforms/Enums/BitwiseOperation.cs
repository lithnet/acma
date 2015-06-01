using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "triggers", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum BitwiseOperation
    {
        [EnumMember(Value="and")]
        And,

        [EnumMember(Value = "or")]
        Or,

        [EnumMember(Value = "nand")]
        Nand,

        [EnumMember(Value = "xor")]
        Xor
    }
}
