using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.Service
{
    [DataContract]
    public class ExportResult
    {
        [DataMember]
        public Guid CSEntryChangeID { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public MAExportError ExportError { get; set; }
    }
}
