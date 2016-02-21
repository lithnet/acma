using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Get, "AcmaObject")]
    public class GetAcmaObjectCmdLet : Cmdlet 
    {
        [Parameter(Mandatory=true,
            HelpMessage="The ID of the AcmaObject",
            HelpMessageBaseName="ID",
            Position=0,
            ValueFromPipeline=true)]
        public Guid ID { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                MAObjectHologram maobject = Global.DataContext.GetMAObjectOrDefault(this.ID);

                if (maobject == null)
                {
                    ErrorRecord error = new ErrorRecord(new NotFoundException(), "ObjectNotFound", ErrorCategory.ObjectNotFound, this.ID);
                    ThrowTerminatingError(error);
                }

                WriteObject(new AcmaPSObject(maobject));
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, this.ID);
                ThrowTerminatingError(error);
            }
        }
    }
}
