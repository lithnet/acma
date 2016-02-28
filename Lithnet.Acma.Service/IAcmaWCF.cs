using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lithnet.Acma;
using System.ServiceModel.Web;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.Service
{
    [ServiceContract]
    public interface IAcmaWCF
    {
        [OperationContract]
        [WebGet(UriTemplate = "/resources/{id}")]
        MAObjectHologram Get(string id);

        [OperationContract]
        [WebGet(UriTemplate = "/csentry/{id}")]
        CSEntryChange GetCSEntryChange(string id);

        [OperationContract]
        [WebGet(UriTemplate = "/resources/{objectType}/{key}/{keyValue}/")]
        MAObjectHologram GetResourceByKey(string objectType, string key, string keyValue);

        [OperationContract]
        [WebGet(UriTemplate = "/watermark")]
        string GetCurrentWatermark();

        [OperationContract]
        [WebGet(UriTemplate = "/resources/?watermark={watermark}")]
        MAObjectHologram[] GetObjects(string watermark);

        [OperationContract]
        [WebGet(UriTemplate = "/resources/{objectType}/")]
        MAObjectHologram[] GetObjectsByClass(string objectType);
    }
}
