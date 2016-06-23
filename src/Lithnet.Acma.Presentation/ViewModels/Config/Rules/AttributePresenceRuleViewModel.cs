using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class AttributePresenceRuleViewModel : RuleViewModel
    {
        private AttributePresenceRule typedModel;

        public AttributePresenceRuleViewModel(AttributePresenceRule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;
        }

        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }
        
        private string GetDisplayName()
        {
            if (this.Attribute == null || (this.IsReferenced && this.ReferencedObjectAttribute == null))
            {
                return string.Format("Undefined attribute presence rule");
            }
            else
            {
                string attributeName = this.IsReferenced ? this.ReferencedObjectAttribute.Name + "->" + this.Attribute.Name : this.Attribute.Name;

                if (this.View == HologramView.Current)
                {
                    attributeName = "#" + attributeName;
                }

                if (this.Operator == PresenceOperator.IsPresent)
                {
                    return string.Format("If {{{0}}} is present", attributeName);
                }
                else
                {
                    return string.Format("If {{{0}}} is not present", attributeName);
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

        public bool IsViewEnabled
        {
            get
            {
                return !this.IsReferenced;
            }
        }

        public PresenceOperator Operator
        {
            get
            {
                return this.typedModel.Operator;
            }
            set
            {
                this.typedModel.Operator = value;
            }
        }


        [PropertyChanged.DependsOn("IsReferenced", "IsLocal")]
        public IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                if (this.IsReferenced)
                {
                    return ActiveConfig.DB.Attributes.OrderBy(t => t.Name);
                }
                else
                {
                    return this.typedModel.ObjectClass.Attributes.OrderBy(t => t.Name);
                }
            }
        }

        [PropertyChanged.DependsOn("IsReferenced", "IsLocal")]
        public IEnumerable<AcmaSchemaAttribute> AllowedReferenceSourceAttributes
        {
            get
            {
                return this.typedModel.ObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Reference && !t.IsMultivalued).OrderBy(t => t.Name);
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

        public HologramView View
        {
            get
            {
                return this.typedModel.View;
            }
            set
            {
                this.typedModel.View = value;
            }
        }

        public AcmaSchemaAttribute ReferencedObjectAttribute
        {
            get
            {
                return this.typedModel.ReferencedObjectAttribute;
            }
            set
            {
                this.typedModel.ReferencedObjectAttribute = value;
            }
        }

        [PropertyChanged.AlsoNotifyFor("IsLocal")]
        public bool IsReferenced
        {
            get
            {
                return this.typedModel.IsReferenced;
            }
            set
            {
                this.typedModel.IsReferenced = value;

                if (value)
                {
                    this.View = HologramView.Current;
                }
                else
                {
                    this.ReferencedObjectAttribute = null;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("IsReferenced")]
        public bool IsLocal
        {
            get
            {
                return !this.IsReferenced;
            }
            set
            {
                this.typedModel.IsReferenced = !value;
            }
        }

        public bool CanSetView
        {
            get
            {
                return this.IsLocal && this.CanUseProposedValues;
            }
        }
    }
}
