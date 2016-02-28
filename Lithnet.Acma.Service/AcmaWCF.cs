using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using System.Runtime.Caching;

namespace Lithnet.Acma.Service
{
    [KnownType(typeof(MAObjectHologram))]
    [KnownType(typeof(List<string>))]
    public class AcmaWCF : IAcmaWCF
    {
        private static MemoryCache cache = new MemoryCache("SearchResultEnumeratorCache");

        public MAObjectHologram Get(string ID)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(new Guid(ID));
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

        public MAObjectHologram GetResourceByKey(string objectType, string key, string keyValue)
        {
            DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), ValueOperator.Equals, keyValue);
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query).FirstOrDefault();
        }

        public string GetCurrentWatermark()
        {
            return ActiveConfig.DB.MADataConext.GetHighWatermarkMAObjects().ToSmartStringOrNull();
        }

        public MAObjectHologram[] GetObjects(string watermark)
        {
            byte[] byteWaterMark = null;

            if (watermark != null)
            {
                byteWaterMark = Convert.FromBase64String(watermark);
            }

            return ActiveConfig.DB.MADataConext.GetMAObjects(byteWaterMark).ToArray();
        }

        public MAObjectHologram[] GetObjectsByClass(string objectType)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(ActiveConfig.DB.GetAttribute("objectClass"), ValueOperator.Equals, objectType).ToArray();
        }

        public ExportResponse ExportObjects(ExportRequest request)
        {
            ExportResponse response = new ExportResponse();
            response.Results = new List<ExportResult>();

            foreach (CSEntryChange csentry in request.CSEntryChanges)
            {
                ExportResult result = new ExportResult();
                result.CSEntryChangeID = csentry.Identifier;

                try
                {
                    bool referenceRetryRequired = false;
                    CSEntryExport.PutExportEntry(csentry, ActiveConfig.DB.MADataConext, out referenceRetryRequired);
                    if (referenceRetryRequired)
                    {
                        result.ExportError = MAExportError.ExportActionRetryReferenceAttribute;
                    }
                }
                catch (Exception ex)
                {
                    result.ExportError = MAExportError.ExportErrorCustomContinueRun;
                        result.Message = ex.Message;
                }

                response.Results.Add(result);
            }

            return response;
        }

        public ImportResponse Import(ImportRequest request)
        {
            ImportResponse response = new ImportResponse();

            if (request.ImportType == OperationType.Delta)
            {
                request.Enumerator = ActiveConfig.DB.MADataConext.EnumerateMAObjectsDelta();
            }
            else
            {
                request.Enumerator = ActiveConfig.DB.MADataConext.EnumerateMAObjects(request.Schema.Types.Select(t => t.Name).ToList(), null, null);
            }

            response.Context = Guid.NewGuid().ToString();
            
            this.AddToCache(response.Context, request);
            this.GetResultPage(response, request.PageSize);

            return response;
        }

        public ImportResponse ImportPage(PageRequest request)
        {
            ImportResponse response = new ImportResponse();
            response.Context = request.Context;

            this.GetResultPage(response, request.PageSize);

            return response;
        }

        private void GetResultPage(ImportResponse response, int pageSize)
        {
            ImportRequest originalRequest = this.GetFromCache(response.Context);

            int itemCount = 0;
            response.Objects = new List<CSEntryChange>();
            response.TotalItems = originalRequest.Enumerator.TotalCount;

            foreach (MAObjectHologram item in originalRequest.Enumerator)
            {
                itemCount++;

                SchemaType type = originalRequest.Schema.Types.FirstOrDefault(t => t.Name == item.ObjectClass.Name);

                if (type == null)
                {
                    continue;
                }

                response.Objects.Add(item.ToCSEntryChange(type));

                if (itemCount >= pageSize)
                {
                    break;
                }
            }

            if (originalRequest.Enumerator.CurrentIndex >= originalRequest.Enumerator.TotalCount - 1)
            {
                this.RemoveFromCache(response.Context);
            }
        }

        private void AddToCache(string context, ImportRequest request)
        {
            AcmaWCF.cache.Add(context, request, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 10, 0) });
        }

        private ImportRequest GetFromCache(string context)
        {
            object item = AcmaWCF.cache.Get(context);

            if (item == null)
            {
                throw new InvalidOperationException("Unknown search context");
            }

            return (ImportRequest)item;
        }

        private void RemoveFromCache(string context)
        {
            AcmaWCF.cache.Remove(context);
        }
    }
}
