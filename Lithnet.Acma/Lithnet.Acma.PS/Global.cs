using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.Acma;
using System.Management.Automation;
using Lithnet.Fim;

namespace Lithnet.Acma.PS
{
    internal static class Global
    {
        public static bool Connected { get; set; }
        public static MADataContext DataContext { get; set; }

        static Global()
        {
            MmsAssemblyResolver.RegisterResolver();
        }

        public static void ThrowIfNotConnected(Cmdlet cmdlet)
        {

            if (!Global.Connected)
            {
                cmdlet.ThrowTerminatingError(new ErrorRecord(new NotConnectedException(), "Call Connect-AcmaEngine before using this cmdlet", ErrorCategory.OpenError, null));
            }
        }
    }
}
