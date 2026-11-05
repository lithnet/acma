using System.Collections.Generic;
using Lithnet.Acma.ServiceModel;

namespace Lithnet.Acma.Service
{
    /// <summary>
    /// The result set of a read started by GetResources, and how far through it the client has paged
    /// </summary>
    internal class ResourceReadContext
    {
        public ResourceReadContext(IList<AcmaResource> resources)
        {
            this.Resources = resources;
        }

        public IList<AcmaResource> Resources { get; private set; }

        public int Position { get; set; }
    }
}
