using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Lithnet.MetadirectoryServices;
using System.Runtime.Serialization;

namespace Lithnet.Acma.Service
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class MmsSurrogateBehavior : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ServiceEndpoint ep in serviceDescription.Endpoints)
            {
                foreach (OperationDescription op in ep.Contract.Operations)
                {
                    DataContractSerializerOperationBehavior dataContractBehavior =
                        op.Behaviors.Find<DataContractSerializerOperationBehavior>()
                        as DataContractSerializerOperationBehavior;
                    if (dataContractBehavior != null)
                    {
                        dataContractBehavior.DataContractSurrogate = new MmsSerializationSurrogate();
                    }
                    else
                    {
                        dataContractBehavior = new DataContractSerializerOperationBehavior(op);
                        dataContractBehavior.DataContractSurrogate = new MmsSerializationSurrogate();
                        op.Behaviors.Add(dataContractBehavior);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}