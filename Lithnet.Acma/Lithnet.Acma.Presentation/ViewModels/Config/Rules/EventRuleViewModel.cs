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
    public class EventRuleViewModel : RuleViewModel
    {
        private EventRule typedModel;

        public EventRuleViewModel(EventRule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;
            this.AnySender = this.typedModel.EventSource == null;
        }

        private string GetDisplayName()
        {
            if (string.IsNullOrWhiteSpace(this.EventName))
            {
                return string.Format("Undefined event rule");
            }
            else
            {
                if (this.EventSource == null)
                {
                    return string.Format("On '{0}' event raised", this.EventName);
                }
                else
                {
                    return string.Format("On '{0}' event raised by {{{1}}}", this.EventName, this.EventSource.Name);
                }
            }
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

        public string EventName
        {
            get
            {
                return this.typedModel.EventName;
            }
            set
            {
                this.typedModel.EventName = value;
            }
        }


        public IEnumerable<AcmaSchemaAttribute> AllowedEventSources
        {
            get
            {
                return this.typedModel.ObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Reference).OrderBy(t => t.Name);
            }
        }

        public bool SourcedEvent { get; set; }

        public bool AnySender
        {
            get
            {
                return this.EventSource == null;
            }
            set
            {
                if (value)
                {
                    this.EventSource = null;
                }
            }
        }

        public bool SpecificSender
        {
            get
            {
                return !this.AnySender;
            }
            set
            {
                this.AnySender = !value;
            }
        }

        public AcmaSchemaAttribute EventSource
        {
            get
            {
                return this.typedModel.EventSource;
            }
            set
            {
                this.typedModel.EventSource = value;
            }
        }
        
    }
}
