using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Fim.Core;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Remove, "AcmaSchemaBinding")]
    public class RemoveAcmaSchemaBinding : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string Attribute { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The object class to bind the attribute to",
            Position = 1,
            ValueFromPipeline = false)]
        public string ObjectClass { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaMapping mapping = ActiveConfig.DB.GetMapping(this.Attribute, this.ObjectClass);
                ActiveConfig.DB.DeleteMapping(mapping);
                ActiveConfig.DB.ClearCache();

                Console.WriteLine("Binding removed");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
