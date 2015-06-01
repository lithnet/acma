using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class AttributeChangeRuleViewModel : RuleViewModel
    {
        private AttributeChangeRule typedModel;

        public AttributeChangeRuleViewModel(AttributeChangeRule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;

            this.ValidatePropertyChange("TriggerEvents");
            this.ValidatePropertyChange("Attribute");
        }

        [PropertyChanged.DependsOn("Attribute", "TriggerEvents", "ReferenceSourceAttribute")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        private string GetDisplayName()
        {
            if (this.Attribute == null)
            {
                return string.Format("Undefined attribute change rule");
            }
            else
            {
                string events;
                string verb;

                if (this.EventAll)
                {
                    verb = "On";
                    events = "changed";
                }
                else if (this.EventNone)
                {
                    verb = "If";
                    events = "is unchanged";
                }
                else
                {
                    verb = "On";
                    events = this.TriggerEvents.ToSmartString().ToLower();
                }

                return string.Format("{0} {{{1}}} {2}", verb, this.Attribute.Name, events);
            }
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                return this.typedModel.ObjectClass.Attributes.OrderBy(t => t.Name);
            }
        }


        public IEnumerable<AcmaSchemaAttribute> AllowedReferenceSourceAttributes
        {
            get
            {
                return this.typedModel.ObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Reference).OrderBy(t => t.Name);
            }
        }

        public AcmaSchemaAttribute Attribute
        {
            get
            {
                return this.typedModel.Attribute;
            }
            set
            {
                this.typedModel.Attribute = value;
            }
        }

        [PropertyChanged.AlsoNotifyFor("EventAdd", "EventDelete", "EventUpdate", "EventAll", "EventNone")]
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

        [PropertyChanged.AlsoNotifyFor("EventAll", "EventAdd", "EventDelete", "EventUpdate")]
        public bool EventNone
        {
            get
            {
                return this.TriggerEvents == TriggerEvents.None;
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = TriggerEvents.None;
                }
                else
                {
                    this.TriggerEvents = TriggerEvents.Add;
                }
            }
        }

        public bool EventAll
        {
            get
            {
                return this.TriggerEvents == (Acma.TriggerEvents.Add | Acma.TriggerEvents.Delete | Acma.TriggerEvents.Update);
            }
            set
            {
                if (value == true)
                {
                    this.TriggerEvents = Acma.TriggerEvents.Add | Acma.TriggerEvents.Delete | Acma.TriggerEvents.Update;
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

            if (this.typedModel == null)
            {
                return;
            }

            if (propertyName == "TriggerEvents")
            {
                if (this.TriggerEvents == 0)
                {
                    this.TriggerEvents = Acma.TriggerEvents.Add;
                }
                
                if (this.TriggerEvents != Acma.TriggerEvents.None && this.TriggerEvents.HasFlag(TriggerEvents.None))
                {
                    this.AddError("EventNone", "Cannot specify a 'none' event with other event types");
                }
                else
                {
                    this.RemoveError("EventNone");
                }
            }
        }
    }
}
