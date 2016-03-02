using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lithnet.Acma;
using System.ServiceModel.Web;

namespace Lithnet.Acma.ServiceModel
{
    [ServiceContract(Namespace = AcmaServiceConstants.Namespace)]
    public interface IAcmaService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/resource/{id}")]
        AcmaResource GetResourceById(string id);
     
        [OperationContract]
        [WebGet(UriTemplate = "/resource/{objectType}/{key}/{keyValue}")]
        AcmaResource GetResourceByTypeAndKey(string objectType, string key, string keyValue);

        [OperationContract]
        [WebGet(UriTemplate = "/resource/?searchAttribute={key}&searchValue={keyValue}&operator={op}")]
        IList<AcmaResource> GetResourcesByAttributePair(string key, string keyValue, string op = "Equals");

        //[OperationContract]
        //[WebGet(UriTemplate = "/resource/?searchAttribute1={key1}&searchValue1={keyValue1}&searchAttribute2={key2}&searchValue2={keyValue2}")]
        //IList<AcmaResource> GetResourcesByAttributePairs(string key1, string keyValue1, string key2, string keyValue2);

        [OperationContract]
        [WebInvoke(UriTemplate = "/resource/{id}", Method="PUT")]
        void ReplaceResource(string id, AcmaResource resource);

        [OperationContract]
        [WebInvoke(UriTemplate = "/resource/{id}", Method = "PATCH")]
        void PatchResource(string id, AcmaResource resource);

        [OperationContract]
        [WebInvoke(UriTemplate = "/resource/{id}", Method = "DELETE")]
        void DeleteResource(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/resource", Method = "POST")]
        void CreateResource(AcmaResource resource);
    }
}
