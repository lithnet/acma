using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma.ServiceModel;

namespace Lithnet.Acma.Service
{
    internal class CachedImportRequest
    {
        public ResultEnumerator Enumerator { get; set; }

        public ImportStartRequest Request { get; set; }

        public byte[] HighWatermark { get; set; }
    }
}
