using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Schema;

namespace Lithnet.Fim.Transforms
{
    [DataContract(Name = "lookup-item", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public class LookupItem: IExtensibleDataObject
    {
        public ExtensionDataObject ExtensionData { get; set; }

        [DataMember(Name = "current-value")]
        public string CurrentValue { get; set; }

        [DataMember(Name = "new-value")]
        public string NewValue { get; set; }
    }
}
