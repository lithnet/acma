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

namespace Lithnet.Acma.Service
{
    [MmsSurrogateBehavior]
    [MmsSurrogateExporter]
    [ServiceBehavior(Namespace = Constants.Namespace)]
    public class AcmaWCF : IAcmaWCF
    {
        private static MemoryCache cache = new MemoryCache("SearchResultEnumeratorCache");

        internal static ManualResetEvent ConfigLock = new ManualResetEvent(true);

        public AcmaResource Get(string ID)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(new Guid(ID)).ToAcmaResource();
        }

        public CSEntryChange GetCSEntryChange(string ID)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(new Guid(ID)).CreateCSEntryChangeFromMAObjectHologram();
        }

        public CSEntryChange GetCSEntryChange(string objectType, string key, string keyValue)
        {
            DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), ValueOperator.Equals, keyValue);
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query).FirstOrDefault().CreateCSEntryChangeFromMAObjectHologram();
        }

        public AcmaResource GetResourceByKey(string objectType, string key, string keyValue)
        {
            DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), ValueOperator.Equals, keyValue);
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query).FirstOrDefault().ToAcmaResource();
        }

        public string GetCurrentWatermark()
        {
            return ActiveConfig.DB.MADataConext.GetHighWatermarkMAObjects().ToSmartStringOrNull();
        }

        public AcmaResource[] GetObjects(string watermark)
        {
            byte[] byteWaterMark = null;

            if (watermark != null)
            {
                byteWaterMark = Convert.FromBase64String(watermark);
            }

            return ActiveConfig.DB.MADataConext.GetMAObjects(byteWaterMark).Select(t => t.ToAcmaResource()).ToArray();
        }

        public AcmaResource[] GetObjectsByClass(string objectType)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(ActiveConfig.DB.GetAttribute("objectClass"), ValueOperator.Equals, objectType).Select(t => t.ToAcmaResource()).ToArray();
        }

        public void ExportStart()
        {
            Logger.WriteSeparatorLine('*');
            Logger.WriteLine("Starting Export");
            AcmaWCF.ConfigLock.Reset();

            MAStatistics.StartOperation(MAOperationType.Export);
            this.ProcessOperationEvents(AcmaEventOperationType.Export);
        }

        public ExportResponse ExportPage(ExportRequest request)
        {
            ExportResponse response = new ExportResponse();
            response.Results = new List<CSEntryChangeResult>();
            IList<AttributeChange> anchorchanges;

            foreach (CSEntryChange csentryChange in request.CSEntryChanges)
            {
                try
                {
                    bool referenceRetryRequired;
                    anchorchanges = CSEntryExport.PutExportEntry(csentryChange, ActiveConfig.DB.MADataConext, out referenceRetryRequired);

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

        public void ExportEnd()
        {
            Logger.WriteLine("Export Complete");
            Logger.WriteSeparatorLine('*');

            if (AcmaWCF.ConfigLock != null)
            {
                AcmaWCF.ConfigLock.Set();
            }

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

            byte[] watermark = ActiveConfig.DB.MADataConext.GetHighWatermarkMAObjectsDelta();
            response.Watermark = watermark == null ? null : Convert.ToBase64String(watermark);
            cachedRequest.HighWatermark = watermark;

            Logger.WriteLine("Got delta watermark: {0}", response.Watermark);

            if (cachedRequest.Request.ImportType == OperationType.Delta)
            {
                this.ProcessOperationEvents(AcmaEventOperationType.DeltaImport);
                cachedRequest.Enumerator = ActiveConfig.DB.MADataConext.EnumerateMAObjectsDelta(watermark);
            }
            else
            {
                this.ProcessOperationEvents(AcmaEventOperationType.FullImport);
                cachedRequest.Enumerator = ActiveConfig.DB.MADataConext.EnumerateMAObjects(cachedRequest.Request.Schema.Types.Select(t => t.Name).ToList(), null, null);
            }

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

            foreach (AcmaOperationEvent e in events.OfType<AcmaOperationEvent>().Where(t => t.OperationTypes.HasFlag(operationType)))
            {
                foreach (MAObjectHologram hologram in e.GetQueryRecipients(ActiveConfig.DB.MADataConext))
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
                catch(InvalidOperationException)
                {
                    Logger.WriteLine("Could not release the request as it was not found in the cache");
                    return;
                }

                if (originalRequest.HighWatermark != null)
                {
                    if (request.NormalTermination)
                    {
                        ActiveConfig.DB.MADataConext.ClearDeltas(originalRequest.HighWatermark);
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
            response.TotalItems = cachedRequest.Enumerator.TotalCount;

            if (pageSize > 0)
            {
                foreach (MAObjectHologram item in cachedRequest.Enumerator)
                {
                    itemCount++;
                    string objectClassName = item.ObjectClass == null ? item.DeltaObjectClassName : item.ObjectClass.Name;
                    SchemaType type = cachedRequest.Request.Schema.Types.FirstOrDefault(t => t.Name == objectClassName);

                    if (type == null)
                    {
                        continue;
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

                    response.Objects.Add(csentry);

                    if (itemCount >= pageSize)
                    {
                        break;
                    }
                }
            }

            if (cachedRequest.Enumerator.CurrentIndex >= cachedRequest.Enumerator.TotalCount)
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
            AcmaWCF.cache.Add(context, request, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 10, 0) });
        }

        private CachedImportRequest GetFromCache(string context)
        {
            object item = AcmaWCF.cache.Get(context);

            if (item == null)
            {
                throw new InvalidOperationException("Unknown search context");
            }

            return (CachedImportRequest)item;
        }

        private void RemoveFromCache(string context)
        {
            AcmaWCF.cache.Remove(context);
        }
    }
}
