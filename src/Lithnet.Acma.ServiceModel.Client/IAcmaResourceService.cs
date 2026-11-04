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

        /// <summary>
        /// Reads the objects of one class that have not been deleted, one page at a time, in object ID order
        /// </summary>
        /// <param name="objectType">The object class to read</param>
        /// <param name="attributes">The attributes to return for each object. objectId and objectClass are always returned</param>
        /// <param name="pageSize">The number of objects per page, from 1 to AcmaServiceConstants.MaximumPageSize</param>
        /// <param name="continuationToken">Null for the first page; otherwise the ContinuationToken of the previous page</param>
        /// <returns>The page of resources, and the token for the next page, or null when there are no more</returns>
        [OperationContract]
        AcmaResourcePage GetResources(string objectType, IList<string> attributes, int pageSize, string continuationToken);

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
