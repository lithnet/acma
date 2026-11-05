using System.Collections.Generic;
using Lithnet.Acma.ServiceModel;

namespace Lithnet.Acma.Service
{
    /// <summary>
    /// The result set of a read started by GetResources, what the read asked for, and how far through it the client has paged
    /// </summary>
    internal class ResourceReadContext
    {
        public ResourceReadContext(string objectType, IList<string> attributeNames, IList<AcmaResource> resources)
        {
            this.ObjectType = objectType;
            this.AttributeNames = attributeNames;
            this.Resources = resources;
        }

        public string ObjectType { get; private set; }

        public IList<string> AttributeNames { get; private set; }

        public IList<AcmaResource> Resources { get; private set; }

        public int Position { get; set; }
    }
}
