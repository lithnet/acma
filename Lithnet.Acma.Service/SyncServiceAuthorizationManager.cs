using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;

namespace Lithnet.Acma.Service
{
    public class SyncServiceAuthorizationManager : ServiceAuthorizationManager
    {
        private const string SyncUsersGroupName = "AcmaSyncUsers";

        private static IdentityReference syncServiceAccount;

        static SyncServiceAuthorizationManager()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\FIMSynchronizationService");

            if (key != null)
            {
                string syncAccountName = (string)key.GetValue("ObjectName", null);

                if (syncAccountName != null)
                {
                    NTAccount t = new NTAccount(syncAccountName);
                    syncServiceAccount = t.Translate(typeof(SecurityIdentifier));
                }
            }
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // Allow MEX requests through. 
            if (operationContext.EndpointDispatcher.ContractName == ServiceMetadataBehavior.MexContractName &&
                operationContext.EndpointDispatcher.ContractNamespace == "http://schemas.microsoft.com/2006/04/mex" &&
                operationContext.IncomingMessageHeaders.Action == "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get")
            {
                return true;
            }

            return (operationContext.ServiceSecurityContext.WindowsIdentity.User == syncServiceAccount);

            //IPrincipal wp = new WindowsPrincipal(operationContext.ServiceSecurityContext.WindowsIdentity);

            //return wp.IsInRole(SyncServiceAuthorizationManager.SyncUsersGroupName);
        }
    }
}
