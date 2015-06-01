using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Logging;
using Lithnet.Fim;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommunications.Disconnect, "AcmaEngine")]
    public class DisconnectAcmaEngine : Cmdlet 
    {
        public DisconnectAcmaEngine()
        {
            MmsAssemblyResolver.RegisterResolver();
        }
        
        protected override void ProcessRecord()
        {
            try
            {
                ActiveConfig.DB = null;
                ActiveConfig.XmlConfig = null;
                Global.Connected = false;
                Global.DataContext = null;
                ActiveConfig.DB.CanCache = true;
                WriteCommandDetail("Disconnected from server");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.OpenError, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
