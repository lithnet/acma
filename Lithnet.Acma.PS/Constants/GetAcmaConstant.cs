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
    [Cmdlet(VerbsCommon.Get, "AcmaConstant")]
    public class GetAcmaConstant : Cmdlet
    {
        [Parameter(
            Mandatory = false,
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
                    foreach (AcmaConstant constant in ActiveConfig.DB.Constants)
                    {
                        this.WriteObject(constant);
                    }
                }
                else
                {
                    this.WriteObject(ActiveConfig.DB.GetConstant(this.Name));
                }
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
