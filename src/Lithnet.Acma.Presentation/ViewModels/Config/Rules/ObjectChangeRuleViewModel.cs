using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.Presentation
{
    public class ObjectChangeRuleViewModel : RuleViewModel
    {
        private ObjectChangeRule typedModel;

        public ObjectChangeRuleViewModel(ObjectChangeRule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;
        }

        private string GetDisplayName()
        {
            string events;

            if (this.EventAll)
            {
                events = "changed";
            }
            else
            {
                events = this.TriggerEvents.ToSmartString().ToLower();
            }

            return string.Format("On object {0}", events);
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }


        [PropertyChanged.AlsoNotifyFor("EventAdd", "EventDelete", "EventUpdate", "EventUndelete")]
        public TriggerEvents TriggerEvents
        {
            get
            {
                return this.typedModel.TriggerEvents;
            }
            set
            {
                this.typedModel.TriggerEvents = value;
            }
        }

        [PropertyChanged.AlsoNotifyFor("EventAll")]
        public bool EventAdd
        {
            get
            {
                return this.TriggerEvents.HasFlag(TriggerEvents.Add);
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = this.TriggerEvents | Acma.TriggerEvents.Add;
                }
                else
                {
                    this.TriggerEvents = this.TriggerEvents & ~Acma.TriggerEvents.Add;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("EventAll")]
        public bool EventDelete
        {
            get
            {
                return this.TriggerEvents.HasFlag(TriggerEvents.Delete);
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = this.TriggerEvents | Acma.TriggerEvents.Delete;
                }
                else
                {
                    this.TriggerEvents = this.TriggerEvents & ~Acma.TriggerEvents.Delete;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("EventAll")]
        public bool EventUpdate
        {
            get
            {
                return this.TriggerEvents.HasFlag(TriggerEvents.Update);
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = this.TriggerEvents | Acma.TriggerEvents.Update;
                }
                else
                {
                    this.TriggerEvents = this.TriggerEvents & ~Acma.TriggerEvents.Update;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("EventAll")]
        public bool EventUndelete
        {
            get
            {
                return this.TriggerEvents.HasFlag(TriggerEvents.Undelete);
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = this.TriggerEvents | Acma.TriggerEvents.Undelete;
                }
                else
                {
                    this.TriggerEvents = this.TriggerEvents & ~Acma.TriggerEvents.Undelete;
                }
            }
        }

        public bool EventAll
        {
            get
            {
                return this.TriggerEvents == (Acma.TriggerEvents.Add | Acma.TriggerEvents.Delete | Acma.TriggerEvents.Update | Acma.TriggerEvents.Undelete);
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = Acma.TriggerEvents.Add | Acma.TriggerEvents.Delete | Acma.TriggerEvents.Update | Acma.TriggerEvents.Undelete;
                }
                else
                {
                    this.TriggerEvents = Acma.TriggerEvents.Add;
                }
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if(propertyName == "TriggerEvents")
            {
                if (this.TriggerEvents == 0)
                {
                    this.TriggerEvents = Acma.TriggerEvents.Add;
                }
            }
        }
    }
}
