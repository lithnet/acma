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
    [Cmdlet(VerbsCommon.Remove, "AcmaSchemaAttribute")]
    public class RemoveAcmaSchemaAttribute : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(this.Name);

                ActiveConfig.DB.DeleteAttribute(attribute);
                ActiveConfig.DB.ClearCache();

                Console.WriteLine("Attribute removed");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
