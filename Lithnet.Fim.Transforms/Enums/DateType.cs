using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "date-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum DateType
    {
        [Description(".NET ticks")]
        [EnumMember(Value = "ticks")]
        Ticks,

        [Description("FIM service date string")]
        [EnumMember(Value = "fim-service-date")]
        FimServiceString,

        [Description("FIM service date string (zeroed milliseconds)")]
        [EnumMember(Value = "fim-service-date-truncated")]
        FimServiceStringTruncated,

        [Description("Custom string format")]
        [EnumMember(Value = "string")]
        String,

        [Description("Native DateTime")]
        [EnumMember(Value = "datetime")]
        DateTime,

        [Description("File time")]
        [EnumMember(Value = "filetime")]
        FileTime
    }
}
