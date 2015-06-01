using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "domain-format", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum DomainFormat
    {
        [Description("Return the domain SID as a string")]
        [EnumMember(Value = "domain-sid-string")]
        DomainSidString,

        [Description("Return the domain SID as a binary value")]
        [EnumMember(Value = "domain-sid-binary")]
        DomainSidBinary,

        [Description("Convert to domain name (if known)")]
        [EnumMember(Value = "domain-name")]
        DomainName
    }
}
