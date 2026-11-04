using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lithnet.Acma;

namespace Lithnet.Acma.ServiceModel
{
    [ServiceContract(Namespace = AcmaServiceConstants.ResourcesNamespace)]
    public interface IAcmaResourceService
    {
        [OperationContract]
        AcmaResource GetResourceById(string id);
     
        [OperationContract]
        AcmaResource GetResourceByTypeAndKey(string objectType, string key, string keyValue);

        [OperationContract]
        IList<AcmaResource> GetResourcesByAttributePair(string key, string keyValue, string op = "Equals");

        [OperationContract]
        void ReplaceResource(string id, AcmaResource resource);

        [OperationContract]
        void PatchResource(string id, AcmaResource resource);

        [OperationContract]
        void DeleteResource(string id);

        [OperationContract]
        void CreateResource(AcmaResource resource);
    }
}
