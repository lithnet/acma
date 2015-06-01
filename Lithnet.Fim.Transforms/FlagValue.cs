using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "flag-value", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public class FlagValue
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public long Value { get; set; }
    }
}
