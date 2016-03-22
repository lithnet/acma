using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma.ServiceModel;
using System.Collections.Concurrent;
using Microsoft.MetadirectoryServices;
using System.Threading;

namespace Lithnet.Acma.Service
{
    internal class CachedImportRequest
    {
        public bool ProducerComplete { get; set; }

        public int Count { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public ConcurrentQueue<CSEntryChange> Queue { get; set; }

        public ImportStartRequest Request { get; set; }

        public byte[] HighWatermark { get; set; }
    }
}
