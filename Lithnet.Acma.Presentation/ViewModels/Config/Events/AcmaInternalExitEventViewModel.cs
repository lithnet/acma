using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class AcmaInternalExitEventViewModel : AcmaEventViewModel
    {
        private AcmaInternalExitEvent typedModel;

        private RuleGroupViewModel ruleGroup;

        private DBQueryObjectsViewModel queryGroups;

        private AcmaSchemaAttributesViewModel allowedRecipients;

        public AcmaInternalExitEventViewModel(AcmaInternalExitEvent model)
            : base(model)
        {
            this.Commands.AddItem("Add", t => this.AddAttribute(), t => this.CanAddAttribute());
            this.Commands.AddItem("Remove", t => this.RemoveAttribute(), t => this.CanRemoveAttribute());
            this.typedModel = model;
            this.RuleGroup = new RuleGroupViewModel(this.typedModel.RuleGroup, false, "Execution rules");
            this.QueryGroups = new DBQueryObjectsViewModel(this.typedModel.RecipientQueries, "Recipient queries");
            this.Recipients = new AcmaSchemaAttributesViewModel(new BindingList<AcmaSchemaAttribute>(this.typedModel.Recipients));
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.RuleGroup;
                yield return this.QueryGroups;
            }
        }

        private bool CanAddAttribute()
        {
            return this.AllowedRecipients.Any(t => t.IsSelected);
        }

        private void AddAttribute()
        {
            ViewModelBase<AcmaSchemaAttribute> vm = this.AllowedRecipients.FirstOrDefault(t => t.IsSelected);

            if (vm != null)
            {
                this.Recipients.Add(vm.Model);
                this.AllowedRecipients.Remove(vm.Model);
            }
        }

        private bool CanRemoveAttribute()
        {
            return this.Recipients.Any(t => t.IsSelected);
        }

        private void RemoveAttribute()
        {
            ViewModelBase<AcmaSchemaAttribute> vm = this.Recipients.FirstOrDefault(t => t.IsSelected);

            if (vm != null)
            {
                this.Recipients.Remove(vm.Model);
                this.AllowedRecipients.Add(vm.Model);
            }
        }

        public RuleGroupViewModel RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }
            set
            {
                if (this.ruleGroup != null)
                {
                    this.UnregisterChildViewModel(this.ruleGroup);
                }

                this.ruleGroup = value;

                if (this.ruleGroup != null)
                {
                    this.RegisterChildViewModel(this.ruleGroup);
                }
            }
        }

        public DBQueryObjectsViewModel QueryGroups
        {
            get
            {
                return this.queryGroups;
            }
            set
            {
                if (this.queryGroups != null)
                {
                    this.UnregisterChildViewModel(this.queryGroups);
                }

                this.queryGroups = value;

                if (this.queryGroups != null)
                {
                    this.RegisterChildViewModel(this.queryGroups);
                }
            }
        }

        public AcmaSchemaAttributesViewModel Recipients { get; set; }

        public AcmaSchemaAttributesViewModel AllowedRecipients
        {
            get
            {
                if (this.allowedRecipients == null)
                {
                    this.allowedRecipients = new AcmaSchemaAttributesViewModel(new BindingList<AcmaSchemaAttribute>(this.typedModel.ObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Reference).ToList()));
                }

                return this.allowedRecipients;
            }
        }
    }
}
