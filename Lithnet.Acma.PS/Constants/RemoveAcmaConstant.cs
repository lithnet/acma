using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Remove, "AcmaConstant")]
    public class RemoveAcmaConstant : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the constant",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                if (string.IsNullOrWhiteSpace(this.Name))
                {
                    throw new ArgumentException("Name");
                }

                ActiveConfig.DB.DeleteConstant(this.Name);

                ActiveConfig.DB.ClearCache();
                Console.WriteLine("Constant deleted");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
