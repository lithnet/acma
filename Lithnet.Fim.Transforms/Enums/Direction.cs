using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "direction", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum Direction
    {
        [EnumMember(Value = "left")]
        Left,

        [EnumMember(Value = "right")]
        Right
    }
}
