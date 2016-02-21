using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using Lithnet.Logging;

namespace Lithnet.Acma.PS
{
    public class AcmaPSObject : IDisposable
    {
        private MAObjectHologram hologram;

        private AcmaObjectAttributeCollection attributes;

        internal AcmaPSObject(MAObjectHologram hologram)
        {
            if (hologram == null)
            {
                throw new ArgumentNullException("hologram");
            }

            this.Events = new List<string>();
            this.hologram = hologram;
            this.attributes = new AcmaObjectAttributeCollection(hologram);
        }

        internal MAObjectHologram InternalHologram
        {
            get
            {
                return this.hologram;
            }
        }

        public string ObjectClass
        {
            get
            {
                return this.hologram.ObjectClass.Name;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.hologram.DisplayText;
            }
        }

        public bool Deleted
        {
            get
            {
                return this.hologram.DeletedTimestamp > 0;
            }
        }

        public bool IsShadowObject
        {
            get
            {
                return this.hologram.ShadowParent != null;
            }
        }

        public Guid ShadowParentID
        {
            get
            {
                return this.hologram.ShadowParent == null ? Guid.Empty : this.hologram.ShadowParent.Id;
            }
        }

        public Guid ObjectID
        {
            get
            {
                return this.hologram.Id;
            }
        }

        public AcmaObjectAttributeCollection Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        private List<string> Events { get; set; }

        public override string ToString()
        {
            return this.hologram.AttributeDataToString();
        }

        public void AddEvent(string addEvent)
        {
            this.Events.Add(addEvent);
        }

        public void Commit()
        {
            if (this.hologram.AcmaModificationType == TriggerEvents.Unconfigured)
            {
                this.hologram.SetObjectModificationType(TriggerEvents.Update);
            }

            try
            {
                this.hologram.CommitCSEntryChange(this.RaiseEvents());
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }
            finally
            {
                this.Events.Clear();
            }
        }

        public void Commit(string[] constructorOverrides)
        {
            if (this.hologram.AcmaModificationType == TriggerEvents.Unconfigured)
            {
                this.hologram.SetObjectModificationType(TriggerEvents.Update);
            }

            this.hologram.CommitCSEntryChange(constructorOverrides.ToList(), this.RaiseEvents());
            this.Events.Clear();
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

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.hologram = null;
                }

                // shared cleanup logic
                disposed = true;
            }
        }

        ~AcmaPSObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}