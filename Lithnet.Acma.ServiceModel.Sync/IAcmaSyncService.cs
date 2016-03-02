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
    [ServiceContract(Namespace = AcmaSyncServiceConstants.Namespace)]
    [MmsSurrogateExporter]
    [MmsSurrogateBehavior]
    public interface IAcmaSyncService
    {
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
    }
}
