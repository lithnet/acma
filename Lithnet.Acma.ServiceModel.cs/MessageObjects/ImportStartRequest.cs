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
    public class ImportStartRequest
    {
        [DataMember]
        public Schema Schema { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string FromWatermark { get; set; }

        [DataMember]
        public OperationType ImportType { get; set; }
    }
}
