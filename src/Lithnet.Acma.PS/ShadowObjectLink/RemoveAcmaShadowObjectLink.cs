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
    [Cmdlet(VerbsCommon.Remove, "AcmaShadowObjectLink")]
    public class RemoveAcmaShadowObjectLink : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the shadow link",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaShadowObjectLink link = ActiveConfig.DB.GetShadowLink(this.Name);
                ActiveConfig.DB.DeleteShadowLink(link);
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
