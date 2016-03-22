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
    [Cmdlet(VerbsCommon.Remove, "AcmaObject")]
    public class RemoveAcmaObject : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The ID of the AcmaObject",
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "ObjectByID")]
        public Guid ID { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "An AcmaObject passed from the pipeline",
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "ObjectByObjectPipeline")]
        public AcmaPSObject AcmaObject { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Indicates if the object should be permanently deleted from the ACMA database",
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public bool ForceDelete { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaPSObject maobject;

                if (this.AcmaObject == null)
                {
                    maobject = new AcmaPSObject(ActiveConfig.DB.GetMAObjectOrDefault(this.ID));
                }
                else
                {
                    maobject = this.AcmaObject;
                }

                if (maobject == null)
                {
                    ErrorRecord error = new ErrorRecord(new NotFoundException(), "ObjectNotFound", ErrorCategory.ObjectNotFound, this.ID);
                    ThrowTerminatingError(error);
                }

                maobject.Hologram.SetObjectModificationType(TriggerEvents.Delete);
                maobject.Hologram.CommitCSEntryChange();

                if (this.ForceDelete)
                {
                    ActiveConfig.DB.DeleteMAObjectPermanent(maobject.Hologram.ObjectID);
                }
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, this.ID);
                ThrowTerminatingError(error);
            }
        }
    }
}
