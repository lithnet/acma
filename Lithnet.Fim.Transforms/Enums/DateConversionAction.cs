using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "date-conversion-action", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum DateConversionAction
    {
        [Description("No conversion")]
        [EnumMember(Value = "none")]
        None = 0,

        [Description("Convert to local time")]
        [EnumMember(Value = "to-local")]
        ToLocal,

        [Description("Convert to UTC")]
        [EnumMember(Value = "to-utc")]
        ToUniversal
    }
}
