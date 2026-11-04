using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Lithnet.Acma.Service
{
    /// <summary>
    /// Adds the <see cref="AcmaServiceErrorHandler"/> to every channel dispatcher of a service host
    /// </summary>
    public class AcmaServiceErrorBehavior : IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher dispatcher = dispatcherBase as ChannelDispatcher;

                if (dispatcher != null)
                {
                    dispatcher.ErrorHandlers.Add(new AcmaServiceErrorHandler());
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
