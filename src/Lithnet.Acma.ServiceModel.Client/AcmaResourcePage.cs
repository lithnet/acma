using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lithnet.Acma.ServiceModel
{
    /// <summary>
    /// One page of resources returned by IAcmaResourceService.GetResources or IAcmaResourceService.GetResourcesPage
    /// </summary>
    [DataContract(Namespace = AcmaServiceConstants.ResourcesNamespace)]
    public class AcmaResourcePage
    {
        /// <summary>
        /// Gets or sets the resources in the page
        /// </summary>
        [DataMember]
        public List<AcmaResource> Resources { get; set; }

        /// <summary>
        /// Gets or sets the token to pass to GetResourcesPage to read the next page, or null when the read has no more pages
        /// </summary>
        [DataMember]
        public string ContinuationToken { get; set; }
    }
}
