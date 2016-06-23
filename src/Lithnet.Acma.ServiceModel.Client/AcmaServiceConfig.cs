using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Lithnet.Acma.ServiceModel
{
    public class AcmaServiceConfig
    {
        public const string NetTcpUri = "net.tcp://localhost:44889/acma/client";

        public const string NetTcpHostnamePlaceHolderUri = "net.tcp://{0}:44889/acma/client";

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

        public static Binding NetTcpBinding
        {
            get
            {
                NetTcpBinding binding = new NetTcpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                binding.CloseTimeout = new TimeSpan(0, 1, 0);
                binding.OpenTimeout = new TimeSpan(0, 1, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 60, 0);
                binding.SendTimeout = new TimeSpan(0, 60, 0);
                binding.TransactionFlow = false;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                
                return binding;
            }
        }

        public static EndpointAddress NetTcpEndpointAddress
        {
            get
            {
                return new EndpointAddress(AcmaServiceConfig.NetTcpUri);
            }
        }
    }
}