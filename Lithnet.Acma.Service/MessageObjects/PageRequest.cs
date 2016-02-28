using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.Service
{
    [DataContract]
    public class PageRequest
    {
        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string Context { get; set; }
    }
}
