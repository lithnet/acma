using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security.Principal;

namespace Lithnet.Acma.Service
{
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // Allow MEX requests through. 
            if (operationContext.EndpointDispatcher.ContractName == ServiceMetadataBehavior.MexContractName &&
                operationContext.EndpointDispatcher.ContractNamespace == "http://schemas.microsoft.com/2006/04/mex" &&
                operationContext.IncomingMessageHeaders.Action == "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get")
            {
                return true;
            }
 
 			var contextUser = operationContext.ServiceSecurityContext.WindowsIdentity;

            return contextUser.Name == "FIM-DEV1\\idm-us-fim-ss";
        }
    }
}
