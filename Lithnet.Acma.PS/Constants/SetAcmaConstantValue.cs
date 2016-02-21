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
    [Cmdlet(VerbsCommon.Set, "AcmaConstantValue")]
    public class SetAcmaConstantValue : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the constant",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The value to set",
            Position = 1,
            ValueFromPipeline = false)]
        public string Value { get; set; }


        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                ActiveConfig.DB.SetConstant(this.Name, this.Value);
                ActiveConfig.DB.ClearCache();
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
