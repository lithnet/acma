using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace Lithnet.Acma.ServiceModel
{
    public class AcmaServiceClient : ClientBase<IAcmaService>, IAcmaService
    {
        public AcmaServiceClient()
            : base(AcmaServiceConfig.NetTcpBinding, AcmaServiceConfig.NetTcpEndpointAddress)
        {
        }

        public AcmaServiceClient(string hostname)
            : base(AcmaServiceConfig.NetTcpBinding, new EndpointAddress(string.Format(AcmaServiceConfig.NetTcpHostnamePlaceHolderUri, hostname)))
        {
        }

        public AcmaResource GetResourceById(string id)
        {
            return this.Channel.GetResourceById(id);
        }

        public AcmaResource GetResourceByTypeAndKey(string objectType, string key, string keyValue)
        {
            return this.Channel.GetResourceByTypeAndKey(objectType, key, keyValue);
        }

        public IList<AcmaResource> GetResourcesByAttributePair(string key, string keyValue, string op = "Equals")
        {
            return this.Channel.GetResourcesByAttributePair(key, keyValue, op);
        }

        public void ReplaceResource(string id, AcmaResource resource)
        {
            this.Channel.ReplaceResource(id, resource);
        }

        public void PatchResource(string id, AcmaResource resource)
        {
            this.Channel.PatchResource(id, resource);
        }

        public void DeleteResource(string id)
        {
            this.Channel.DeleteResource(id);
        }

        public void CreateResource(AcmaResource resource)
        {
            this.Channel.CreateResource(resource);
        }
    }
}