using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using System.Collections;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsData.Save, "AcmaObject")]
    public class SaveAcmaObjectCmdLet : AcmaCmdletConnected
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, Position = 1)]
        public AcmaPSObject AcmaObject { get; set; }

        [Parameter(Mandatory = false, Position = 2)]
        public string[] ConstructorOverrides { get; set; }

        [Parameter(Mandatory = false, Position = 3)]
        public string[] Events { get; set; }

        protected override void ProcessRecord()
        {
            if (!this.IsConnectionStatusOk(true))
            {
                return;
            }

            if (this.AcmaObject.Hologram.AcmaModificationType == TriggerEvents.Unconfigured)
            {
                this.AcmaObject.Hologram.SetObjectModificationType(TriggerEvents.Update);
            }

            MAObjectHologram hologram = this.AcmaObject.GetResourceWithAppliedChanges();

            if (this.ConstructorOverrides == null)
            {
                hologram.CommitCSEntryChange(this.RaiseEvents());
            }
            else
            {
                hologram.CommitCSEntryChange(this.ConstructorOverrides.ToList(), this.RaiseEvents());
            }

            this.AcmaObject.Refresh();
        }

        private IList<RaisedEvent> RaiseEvents()
        {
            List<RaisedEvent> raisedEvents = new List<RaisedEvent>();

            if (this.Events != null)
            {
                foreach (string eventName in this.Events)
                {
                    AcmaInternalExitEvent internalEvent = new AcmaInternalExitEvent();
                    internalEvent.AllowDuplicateIDs = true;
                    internalEvent.ID = eventName;
                    RaisedEvent raisedEvent = new RaisedEvent(internalEvent);
                    raisedEvents.Add(raisedEvent);
                }
            }

            return raisedEvents;
        }
    }
}