using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace Lithnet.Acma.Service
{
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        private const string AdminGroupName = "AcmaAdministrators";

        private const string SyncUsersGroupName = "AcmaSyncUsers";

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // Allow MEX requests through. 
            if (operationContext.EndpointDispatcher.ContractName == ServiceMetadataBehavior.MexContractName &&
                operationContext.EndpointDispatcher.ContractNamespace == "http://schemas.microsoft.com/2006/04/mex" &&
                operationContext.IncomingMessageHeaders.Action == "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get")
            {
                return true;
            }

            IPrincipal wp = new WindowsPrincipal(operationContext.ServiceSecurityContext.WindowsIdentity);

            return wp.IsInRole(AuthorizationManager.AdminGroupName) || wp.IsInRole(AuthorizationManager.SyncUsersGroupName);
        }
    }
}
