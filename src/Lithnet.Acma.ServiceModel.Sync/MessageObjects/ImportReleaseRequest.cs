using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
using System.Collections;

namespace Lithnet.Acma.ServiceModel
{
    [DataContract]
    public class ImportReleaseRequest
    {
        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public bool NormalTermination { get; set; }
    }
}
