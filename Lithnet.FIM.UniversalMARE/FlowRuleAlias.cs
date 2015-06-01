using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Schema;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.UniversalMARE
{
    [DataContract(Name = "flow-rule-alias", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class FlowRuleAlias : UINotifyPropertyChanges, IExtensibleDataObject
    {
        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "definition")]
        public string FlowRuleDefinition { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
