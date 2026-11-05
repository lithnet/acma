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
        /// Starts a read of the objects of one class that have not been deleted, and returns the first page. Every page of the read
        /// holds the objects and values as they were when this call started, in object ID order
        /// </summary>
        /// <param name="objectType">The object class to read</param>
        /// <param name="attributes">The attributes to return for each object. objectId and objectClass are always returned</param>
        /// <param name="pageSize">The number of objects in the page, from 1 to AcmaServiceConstants.MaximumPageSize</param>
        /// <returns>The first page, and the token for the next page, or null when there are no more</returns>
        [OperationContract]
        AcmaResourcePage GetResources(string objectType, IList<string> attributes, int pageSize);

        /// <summary>
        /// Gets the next page of a read started by GetResources. The read ends after its last page, after any error,
        /// and AcmaServiceConstants.ResourceReadExpiryMinutes after its last page was read
        /// </summary>
        /// <param name="continuationToken">The ContinuationToken of the previous page</param>
        /// <param name="pageSize">The number of objects in the page, from 1 to AcmaServiceConstants.MaximumPageSize</param>
        /// <returns>The page, and the token for the next page, or null when there are no more</returns>
        [OperationContract]
        AcmaResourcePage GetResourcesPage(string continuationToken, int pageSize);

        /// <summary>
        /// Ends a read started by GetResources before its last page
        /// </summary>
        /// <param name="continuationToken">The ContinuationToken of the last page read</param>
        [OperationContract]
        void ReleaseResources(string continuationToken);

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
