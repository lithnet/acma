using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Lithnet.Acma.Service
{
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            
            if ((operationContext.ServiceSecurityContext.IsAnonymous) ||
              (operationContext.ServiceSecurityContext.PrimaryIdentity == null))
            {
                //_logger.Error("WcfWindowsSecurityApplied = true but no credentials have been supplied");
                return false;
            }

            string username = operationContext.ServiceSecurityContext.WindowsIdentity.Name;

            //if (operationContext.ServiceSecurityContext.PrimaryIdentity.Name.ToLower() == Global.WcfCredentialsDomain.ToLower() + "\\" + Global.WcfCredentialsUserName.ToLower())
            //{
            //    // _logger.Debug("WcfOnlyAuthorizedForWcfCredentials = true and the valid user (" + operationContext.ServiceSecurityContext.PrimaryIdentity.Name + ") has been supplied and access allowed");
            //    return true;
            //}
            //else
            //{
            //    //_logger.Error("WcfOnlyAuthorizedForWcfCredentials = true and an invalid user (" + operationContext.ServiceSecurityContext.PrimaryIdentity.Name + ") has been supplied and access denied");
            //    return false;
            //}

            return false;
        }
    }

}
