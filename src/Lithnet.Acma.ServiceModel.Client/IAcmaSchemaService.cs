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
    [ServiceContract(Namespace = AcmaServiceConstants.SchemaNamespace)]
    public interface IAcmaSchemaService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/schema/objectclass/{name}")]
        string GetObjectClass(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/schema/objectclass/{name}/binding/")]
        string GetObjectClassBindings(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/schema/objectclass/{name}/backlink/")]
        string GetObjectClassBackLinks(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/schema/objectclass/{name}/shadowlink/")]
        string GetObjectClassShadowLinks(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/schema/attribute/{name}")]
        string GetAttribute(string name);

        [OperationContract]
        [WebGet(UriTemplate = "/resource/{objectType}/{key}/{keyValue}")]
        AcmaResource GetResourceByTypeAndKey(string objectType, string key, string keyValue);

        [OperationContract]
        [WebGet(UriTemplate = "/resource/?searchAttribute={key}&searchValue={keyValue}&operator={op}")]
        IList<AcmaResource> GetResourcesByAttributePair(string key, string keyValue, string op = "Equals");

        [OperationContract]
        [WebInvoke(UriTemplate = "/resource/{id}", Method="PUT")]
        void ReplaceResource(string id, AcmaResource resource);

        [OperationContract]
        [WebInvoke(UriTemplate = "/resource/{id}", Method = "PATCH")]
        void PatchResource(string id, AcmaResource resource);

        [OperationContract]
        [WebInvoke(UriTemplate = "/schema/objectclass/{name}", Method = "DELETE")]
        void DeleteObjectClass(string name);

        [OperationContract]
        [WebInvoke(UriTemplate = "/schema/attribute/{name}", Method = "DELETE")]
        void DeleteAttribute(string name);

        [OperationContract]
        [WebInvoke(UriTemplate = "/schema/objectclass/{name}/binding/{attribute}", Method = "DELETE")]
        void DeleteBinding(string name, string attribute);

        [OperationContract]
        [WebInvoke(UriTemplate = "/schema/objectclass/", Method = "POST")]
        void CreateObjectClass(object resource);

        [OperationContract]
        [WebInvoke(UriTemplate = "/schema/attribute/", Method = "POST")]
        void CreateAttribute(object resource);

        [OperationContract]
        [WebInvoke(UriTemplate = "/schema/objectclass/{name}/binding/{attribute}", Method = "POST")]
        void CreateBinding(object resource);

    }
}
