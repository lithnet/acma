using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.ServiceModel
{
    [DataContract]
    public class ExportRequest
    {
        [DataMember]
        public IList<CSEntryChange> CSEntryChanges { get; set; }
    }
}
