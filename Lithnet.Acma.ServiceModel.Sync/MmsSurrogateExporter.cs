using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Lithnet.MetadirectoryServices;
using System.Runtime.Serialization;

namespace Lithnet.Acma.ServiceModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class MmsSurrogateExporter : Attribute, IWsdlExportExtension, IContractBehavior
    {
        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            object dataContractExporter;
            XsdDataContractExporter xsdInventoryExporter;
            if (!exporter.State.TryGetValue(typeof(XsdDataContractExporter), out dataContractExporter))
            {
                xsdInventoryExporter = new XsdDataContractExporter(exporter.GeneratedXmlSchemas);
            }
            else
            {
                xsdInventoryExporter = (XsdDataContractExporter)dataContractExporter;
            }

            exporter.State.Add(typeof(XsdDataContractExporter), xsdInventoryExporter);

            if (xsdInventoryExporter.Options == null)
            {
                xsdInventoryExporter.Options = new ExportOptions();
            }

            xsdInventoryExporter.Options.DataContractSurrogate = new MmsSerializationSurrogate();

        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
        }
    }
}