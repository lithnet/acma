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
    [Cmdlet(VerbsCommon.Get, "AcmaSchemaAttribute")]
    public class GetAcmaSchemaAttribute : Cmdlet
    {
        [Parameter(
            Mandatory = false,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    foreach (AcmaSchemaAttribute attribute in ActiveConfig.DB.Attributes)
                    {
                        this.WriteObject(attribute);
                    }
                }
                else
                {
                    AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(this.Name);
                    this.WriteObject(attribute);
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
