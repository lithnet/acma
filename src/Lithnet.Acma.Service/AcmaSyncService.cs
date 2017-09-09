using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using System.Runtime.Caching;
using Lithnet.Acma.DataModel;
using System.ServiceModel;
using Lithnet.Acma.ServiceModel;
using Lithnet.Logging;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Lithnet.Acma.Service
{
    [MmsSurrogateBehavior]
    [MmsSurrogateExporter]
    [ServiceBehavior(Namespace = AcmaSyncServiceConstants.Namespace)]
    public class AcmaSyncService : IAcmaSyncService
    {
        private static MemoryCache cache = new MemoryCache("SearchResultEnumeratorCache");

        public string GetCurrentWatermark()
        {
            return ActiveConfig.DB.GetHighWatermarkMAObjects().ToSmartStringOrNull();
        }

        public void ExportStart()
        {
            Logger.WriteSeparatorLine('*');
            Logger.WriteLine("Starting Export");

            MAStatistics.StartOperation(MAOperationType.Export);
            this.ProcessOperationEvents(AcmaEventOperationType.Export);
        }

        public ExportResponse ExportPage(ExportRequest request)
        {
            try
            {
                Monitor.Enter(ServiceMain.Lock);

                ExportResponse response = new ExportResponse();
                response.Results = new List<CSEntryChangeResult>();
                IList<AttributeChange> anchorchanges;

                foreach (CSEntryChange csentryChange in request.CSEntryChanges)
                {
                    try
                    {
                        bool referenceRetryRequired;
                        anchorchanges = CSEntryExport.PutExportEntry(csentryChange, out referenceRetryRequired);

                        if (referenceRetryRequired)
                        {
                            Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentryChange.DN));
                            response.Results.Add(CSEntryChangeResult.Create(csentryChange.Identifier, anchorchanges, MAExportError.ExportActionRetryReferenceAttribute));
                        }
                        else
                        {
                            response.Results.Add(CSEntryChangeResult.Create(csentryChange.Identifier, anchorchanges, MAExportError.Success));
                        }
                    }
                    catch (Exception ex)
                    {
                        MAStatistics.AddExportError();

                        response.Results.Add(this.GetExportChangeResultFromException(csentryChange, ex));
                    }
                }

                return response;
            }
            finally
            {
                Monitor.Exit(ServiceMain.Lock);
            }

        }

        public void ExportEnd()
        {
            Logger.WriteLine("Export Complete");
            Logger.WriteSeparatorLine('*');
          
            MAStatistics.StopOperation();
            Logger.WriteLine(MAStatistics.ToString());
        }

        /// <summary>
        /// Constructs a CSEntryChangeResult object appropriate to the specified exception
        /// </summary>
        /// <param name="csentryChange">The CSEntryChange object that triggered the exception</param>
        /// <param name="ex">The exception that was caught</param>
        /// <returns>A CSEntryChangeResult object with the correct error code for the exception that was encountered</returns>
        private CSEntryChangeResult GetExportChangeResultFromException(CSEntryChange csentryChange, Exception ex)
        {
            if (ex is NoSuchObjectException)
            {
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportErrorConnectedDirectoryMissingObject);
            }
            else if (ex is ReferencedObjectNotPresentException)
            {
                Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentryChange.DN));
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportActionRetryReferenceAttribute);
            }
            else
            {
                Logger.WriteLine(string.Format("An unexpected exception occurred for csentry change {0} with DN {1}. ", csentryChange.Identifier.ToString(), csentryChange.DN ?? string.Empty));
                Logger.WriteException(ex);
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportErrorCustomContinueRun, ex.Message, ex.StackTrace);
            }
        }

        public ImportResponse ImportStart(ImportStartRequest request)
        {
            Logger.WriteLine("Import request received");
            MAStatistics.StartOperation(MAOperationType.Import);

            ImportResponse response = new ImportResponse();
            CachedImportRequest cachedRequest = new CachedImportRequest();
            cachedRequest.Request = request;

            byte[] watermark = ActiveConfig.DB.GetHighWatermarkMAObjectsDelta();
            response.Watermark = watermark == null ? null : Convert.ToBase64String(watermark);
            cachedRequest.HighWatermark = watermark;

            Logger.WriteLine("Got delta watermark: {0}", response.Watermark);
           
            ResultEnumerator enumerator;

            if (cachedRequest.Request.ImportType == OperationType.Delta)
            {
                this.ProcessOperationEvents(AcmaEventOperationType.DeltaImport);
                enumerator = ActiveConfig.DB.EnumerateMAObjectsDelta(watermark);
            }
            else
            {
                this.ProcessOperationEvents(AcmaEventOperationType.FullImport);
                enumerator = ActiveConfig.DB.EnumerateMAObjects(cachedRequest.Request.Schema.Types.Select(t => t.Name).ToList(), null, null);
            }

            cachedRequest.Queue = new ConcurrentQueue<CSEntryChange>();
            cachedRequest.Count = enumerator.TotalCount;
            this.StartProducerThread(enumerator, request.Schema, cachedRequest);

            response.Context = Guid.NewGuid().ToString();

            this.AddToCache(response.Context, cachedRequest);
            this.GetResultPage(response, request.PageSize);

            return response;
        }

        private void ProcessOperationEvents(AcmaEventOperationType operationType)
        {
            AcmaEvents events = ActiveConfig.XmlConfig.OperationEvents;
            if (events == null || events.Count == 0)
            {
                return;
            }

            Logger.WriteSeparatorLine('-');
            Logger.WriteLine("Processing pre-operation events");

            foreach (AcmaOperationEvent e in events.OfType<AcmaOperationEvent>().Where(t => t.OperationTypes.HasFlag(operationType) && !t.IsDisabled))
            {
                foreach (MAObjectHologram hologram in e.GetQueryRecipients())
                {
                    Logger.WriteLine("Sending event {0} to {1}", e.ID, hologram.DisplayText);
                    try
                    {
                        Logger.IncreaseIndent();
                        hologram.ProcessEvents(new List<RaisedEvent>() { new RaisedEvent(e) });
                    }
                    finally
                    {
                        Logger.DecreaseIndent();
                    }
                }
            }

            Logger.WriteLine("Event distribution completed");
            Logger.WriteSeparatorLine('-');
        }

        public ImportResponse ImportPage(PageRequest request)
        {
            ImportResponse response = new ImportResponse();
            response.Context = request.Context;

            this.GetResultPage(response, request.PageSize);

            return response;
        }

        public void ImportEnd(ImportReleaseRequest request)
        {
            try
            {
                CachedImportRequest originalRequest;

                try
                {
                    originalRequest = GetFromCache(request.Context);
                }
                catch (InvalidOperationException)
                {
                    Logger.WriteLine("Could not release the request as it was not found in the cache");
                    return;
                }

                if (originalRequest.HighWatermark != null)
                {
                    if (request.NormalTermination)
                    {
                        ActiveConfig.DB.ClearDeltas(originalRequest.HighWatermark);
                    }
                }

                RemoveFromCache(request.Context);

                MAStatistics.StopOperation();
                Logger.WriteLine(MAStatistics.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Could not release the request");
                Logger.WriteException(ex);
            }
        }

        public Schema GetMmsSchema()
        {
            Schema schema = Schema.Create();

            foreach (AcmaSchemaObjectClass schemaObject in ActiveConfig.DB.ObjectClassesBindingList)
            {
                SchemaType schemaType = SchemaType.Create(schemaObject.Name, true);
                schemaType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute("objectId", AttributeType.String, AttributeOperation.ImportOnly));

                foreach (AcmaSchemaAttribute attribute in schemaObject.Attributes.Where(t => t.Name != "objectId"))
                {
                    if (attribute.Operation == AcmaAttributeOperation.AcmaInternal || attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
                    {
                        continue;
                    }

                    if (attribute.IsMultivalued)
                    {
                        schemaType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(attribute.Name, attribute.MmsType, attribute.MmsOperationType));
                    }
                    else
                    {
                        schemaType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(attribute.Name, attribute.MmsType, attribute.MmsOperationType));
                    }
                }

                schema.Types.Add(schemaType);
            }

            return schema;
        }

        private void GetResultPage(ImportResponse response, int pageSize)
        {
            CachedImportRequest cachedRequest = this.GetFromCache(response.Context);

            int itemCount = 0;
            response.Objects = new List<CSEntryChange>();
            response.TotalItems = cachedRequest.Count;

            if (pageSize > 0)
            {
                CSEntryChange csentry;

                while (cachedRequest.Queue.TryDequeue(out csentry))
                {
                    itemCount++;

                    response.Objects.Add(csentry);

                    if (itemCount >= pageSize)
                    {
                        break;
                    }
                }
            }

            if (cachedRequest.Queue.IsEmpty && cachedRequest.ProducerComplete)
            {
                response.HasMoreItems = false;
            }
            else
            {
                response.HasMoreItems = true;
            }
        }

        private void AddToCache(string context, CachedImportRequest request)
        {
            AcmaSyncService.cache.Add(context, request, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 10, 0) });
        }

        private CachedImportRequest GetFromCache(string context)
        {
            object item = AcmaSyncService.cache.Get(context);

            if (item == null)
            {
                throw new InvalidOperationException("Unknown search context");
            }

            return (CachedImportRequest)item;
        }

        private void RemoveFromCache(string context)
        {
            AcmaSyncService.cache.Remove(context);
        }

        private void StartProducerThread(ResultEnumerator results, Schema schema, CachedImportRequest request)
        {
            Task t = new Task(() =>
                {
                    this.ProduceCSEntryChanges(results, schema, request);
                });

            t.Start();
        }

        private void ProduceCSEntryChanges(ResultEnumerator results, Schema schema, CachedImportRequest request)
        {
            ParallelOptions op = new ParallelOptions();
            op.CancellationToken = new CancellationToken();
            request.CancellationToken = op.CancellationToken;
            op.MaxDegreeOfParallelism = 1;

            Parallel.ForEach(results, op, item =>
            {
                if (op.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                string objectClassName = item.ObjectClass == null ? item.DeltaObjectClassName : item.ObjectClass.Name;
                SchemaType type = schema.Types.FirstOrDefault(t => t.Name == objectClassName);

                if (type == null)
                {
                    return;
                }

                CSEntryChange csentry = null;

                try
                {
                    csentry = item.ToCSEntryChange(type);
                    MAStatistics.AddImportOperation();
                }
                catch (Exception ex)
                {
                    MAStatistics.AddImportError();

                    if (csentry == null)
                    {
                        csentry = CSEntryChange.Create();
                    }

                    if (string.IsNullOrWhiteSpace(csentry.DN))
                    {
                        csentry.ErrorCodeImport = MAImportError.ImportErrorCustomStopRun;
                    }
                    else
                    {
                        csentry.ErrorCodeImport = MAImportError.ImportErrorCustomContinueRun;
                    }

                    csentry.ErrorName = ex.Message;
                    csentry.ErrorDetail = ex.StackTrace;
                }

                request.Queue.Enqueue(csentry);
            });

            request.ProducerComplete = true;
        }
    }
}
