using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.ServiceModel
{
    [DataContract]
    public class ImportResponse
    {
        [DataMember]
        public IList<CSEntryChange> Objects { get; set; }

        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public int TotalItems { get; set; }

        [DataMember]
        public bool HasMoreItems { get; set; }

        [DataMember]
        public string Watermark { get; set; }
    }
}
