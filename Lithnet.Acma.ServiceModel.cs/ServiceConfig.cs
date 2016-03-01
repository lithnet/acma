using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Lithnet.Acma.ServiceModel
{
    public class ServiceConfig
    {
        public const string NamedPipeUri = "net.pipe://localhost/acma";

        public static ServiceMetadataBehavior ServiceMetadataDisabledBehavior
        {
            get
            {
                return new ServiceMetadataBehavior
                 {
                     HttpGetEnabled = false,
                     HttpsGetEnabled = false
                 };
            }
        }

        public static ServiceDebugBehavior ServiceDebugBehavior
        {
            get
            {
                return new ServiceDebugBehavior
                {
                    IncludeExceptionDetailInFaults = true,
                    HttpHelpPageEnabled = false,
                    HttpsHelpPageEnabled = false
                };
            }
        }

        public static Binding NetNamedPipeBinding
        {
            get
            {
                NetNamedPipeBinding binding = new NetNamedPipeBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                binding.CloseTimeout = new TimeSpan(0, 1, 0);
                binding.OpenTimeout = new TimeSpan(0, 1, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 60, 0);
                binding.SendTimeout = new TimeSpan(0, 60, 0);
                binding.TransactionFlow = false;
                binding.Security.Mode = NetNamedPipeSecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
                return binding;
            }
        }

        public static EndpointAddress NetNamedPipeEndpointAddress
        {
            get
            {
                return new EndpointAddress(ServiceConfig.NamedPipeUri);
            }
        }
    }
}