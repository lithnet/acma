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
    [Cmdlet(VerbsCommon.Rename, "AcmaSchemaAttribute")]
    public class RenameAcmaSchemaAttribute : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The current name of the attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string CurrentName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The new name of the attribute",
            Position = 1,
            ValueFromPipeline = false)]
        public string NewName { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                ActiveConfig.DB.RenameAttribute(this.CurrentName, this.NewName);
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
