using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lithnet.Acma;

namespace Lithnet.Acma.ServiceModel
{
    [ServiceContract(Namespace = AcmaServiceConstants.SchemaNamespace)]
    public interface IAcmaSchemaService
    {
        [OperationContract]
        string GetObjectClass(string name);

        [OperationContract]
        string GetObjectClassBindings(string name);

        [OperationContract]
        string GetObjectClassBackLinks(string name);

        [OperationContract]
        string GetObjectClassShadowLinks(string name);

        [OperationContract]
        string GetAttribute(string name);

        [OperationContract]
        AcmaResource GetResourceByTypeAndKey(string objectType, string key, string keyValue);

        [OperationContract]
        IList<AcmaResource> GetResourcesByAttributePair(string key, string keyValue, string op = "Equals");

        [OperationContract]
        void ReplaceResource(string id, AcmaResource resource);

        [OperationContract]
        void PatchResource(string id, AcmaResource resource);

        [OperationContract]
        void DeleteObjectClass(string name);

        [OperationContract]
        void DeleteAttribute(string name);

        [OperationContract]
        void DeleteBinding(string name, string attribute);

        [OperationContract]
        void CreateObjectClass(object resource);

        [OperationContract]
        void CreateAttribute(object resource);

        [OperationContract]
        void CreateBinding(object resource);
    }
}
