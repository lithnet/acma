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
        private static readonly object lockObject = new object();

        private static IdentityReference syncServiceAccount;

        private static IdentityReference SyncServiceAccount
        {
            get
            {
                if (syncServiceAccount == null)
                {
                    lock (lockObject)
                    {
                        if (syncServiceAccount == null)
                        {
                            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\FIMSynchronizationService");

                            string syncAccountName = (string)key?.GetValue("ObjectName", null);

                            if (syncAccountName != null)
                            {
                                if (syncAccountName.StartsWith(".\\"))
                                {
                                    syncAccountName = syncAccountName.Replace(".\\", Environment.MachineName + "\\");
                                }

                                NTAccount t = new NTAccount(syncAccountName);
                                syncServiceAccount = t.Translate(typeof(SecurityIdentifier));
                            }
                            else
                            {
                                throw new InvalidOperationException("The synchronization service username was not found in the registry");
                            }
                        }
                    }
                }

                return syncServiceAccount;
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

            if (SyncServiceAccount == null)
            {
                return false;
            }

            return operationContext.ServiceSecurityContext.WindowsIdentity.User == SyncServiceAccount;
        }
    }
}
