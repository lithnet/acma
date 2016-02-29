﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.Service
{
    [DataContract]
    public class ExportResponse
    {
        [DataMember]
        public IList<ExportResult> Results { get; set; }
    }
}