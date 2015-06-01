using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "delimiter-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum DelimiterType
    {
        [Description("Comma separated")]
        [EnumMember(Value = "comma")]
        CommaSeparated,

        [Description("Tab separated")]
        [EnumMember(Value = "tab")]
        TabSeparated,

        [Description("Custom")]
        [EnumMember(Value = "custom")]
        Custom
    }
}
