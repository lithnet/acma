using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Fim.Core;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Add, "AcmaObject")]
    public class AddAcmaObject : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The object class to create",
            Position = 0,
            ValueFromPipeline = true)]
        public string ObjectClass { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                MAObjectHologram maobject = Global.DataContext.CreateMAObject(Guid.NewGuid(), this.ObjectClass);
                WriteObject(new AcmaPSObject(maobject));
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
