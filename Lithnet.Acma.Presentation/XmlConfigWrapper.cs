using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using Lithnet.Acma.TestEngine;
using System.Runtime.Serialization;

namespace Lithnet.Acma.Presentation
{
    [DataContract(Name = "acma-config", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class XmlConfigWrapper
    {
        [DataMember(Name = "acma", Order = 0)]
        public XmlConfigFile AcmaConfig { get; set; }

        [DataMember(Name = "acma-unit-tests", Order = 1)]
        public UnitTestFile UnitTests { get; set; }
    }
}
