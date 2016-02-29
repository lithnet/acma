using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lithnet.Acma;
using System.ServiceModel.Web;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.ServiceModel
{
    [ServiceContract(Namespace = Constants.Namespace)]
    [MmsSurrogateExporter]
    [MmsSurrogateBehavior]
    public interface IAcmaWCF
    {
        [OperationContract]
        [WebGet(UriTemplate = "/resources/{id}")]
        AcmaResource Get(string id);

        [OperationContract]
        [WebGet(UriTemplate = "/csentry/{id}")]
        CSEntryChange GetCSEntryChange(string id);

        [OperationContract]
        [WebGet(UriTemplate = "/resources/{objectType}/{key}/{keyValue}/")]
        AcmaResource GetResourceByKey(string objectType, string key, string keyValue);
       
        [OperationContract]
        [WebGet(UriTemplate = "/resources/?watermark={watermark}")]
        AcmaResource[] GetObjects(string watermark);

        [OperationContract]
        [WebGet(UriTemplate = "/resources/{objectType}/")]
        AcmaResource[] GetObjectsByClass(string objectType);

        // Sync engine operations

        [OperationContract]
        [WebGet(UriTemplate = "/sync/watermark")]
        string GetCurrentWatermark();

        [OperationContract]
        [WebInvoke(UriTemplate = "/sync/export/page", Method = "PUT")]
        ExportResponse ExportPage(ExportRequest request);

        [OperationContract]
        [WebGet(UriTemplate = "/sync/export")]
        void ExportStart();

        [OperationContract]
        [WebGet(UriTemplate = "/sync/export/release")]
        void ExportEnd();

        [OperationContract]
        [WebInvoke(UriTemplate = "/sync/import", Method = "POST")]
        ImportResponse ImportStart(ImportStartRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "/sync/import/page", Method = "POST")]
        ImportResponse ImportPage(PageRequest request);

        [OperationContract]
        [WebInvoke(UriTemplate = "/sync/import/release", Method = "POST")]
        void ImportEnd(ImportReleaseRequest request);

        [OperationContract]
        [WebGet(UriTemplate = "/sync/schema")]
        Schema GetMmsSchema();

        // Need a release operation 
    }
}
