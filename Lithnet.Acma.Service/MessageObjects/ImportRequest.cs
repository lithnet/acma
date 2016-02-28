using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.Service
{
    [DataContract]
    public class ImportRequest
    {
        [DataMember]
        public Schema Schema { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public byte[] HighWatermark { get; set; }

        [DataMember]
        public byte[] LowWatermark { get; set; }

        [DataMember]
        public OperationType ImportType { get; set; }

        internal ResultEnumerator Enumerator { get; set; }
    }
}
