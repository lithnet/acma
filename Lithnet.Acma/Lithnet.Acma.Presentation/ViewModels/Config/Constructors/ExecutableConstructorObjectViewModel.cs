using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.Presentation
{
    public abstract class ExecutableConstructorObjectViewModel : ViewModelBase<ExecutableConstructorObject>
    {
        private RuleGroupViewModel ruleGroup;

        public ExecutableConstructorObjectViewModel(ExecutableConstructorObject model)
            : base(model)
        {
            if (this.Model.RuleGroup != null)
            {
                this.RuleGroup = new RuleGroupViewModel(this.Model.RuleGroup, true, "Execution rules");
            }

            this.EnableCutCopy();
            this.Commands.AddItem("AddExecutionConditions", t => this.AddExecutionConditions(), t => this.CanAddExecutionConditions());
            this.Commands.AddItem("RemoveExecutionConditions", t => this.RemoveExecutionConditions(), t => this.CanRemoveExecutionConditions());
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
                    this.ruleGroup.ParentConstructor = this;
                    this.RegisterChildViewModel(this.ruleGroup);
                }
            }
        }

        public abstract string DisplayName { get; }

        public string Description
        {
            get
            {
                return this.Model.Description;
            }
            set
            {
                this.Model.Description = value;
            }
        }

        public string Id
        {
            get
            {
                return this.Model.ID;
            }
            set
            {
                this.Model.ID = value;
                this.RaisePropertyChanged("DisplayName");
            }
        }

        public bool Disabled
        {
            get
            {
                return this.Model.Disabled;
            }
            set
            {
                this.Model.Disabled = value;
                this.RaisePropertyChanged("DisplayIcon");
            }
        }

        public void AddExecutionConditions()
        {
            if (this.RuleGroup == null)
            {
                this.Model.RuleGroup = new RuleGroup();
                RuleGroupViewModel vm = new RuleGroupViewModel(this.Model.RuleGroup, true, "Execution rules");
                this.RuleGroup = vm;
                this.IsExpanded = true;
                this.RuleGroup.IsExpanded = true;
                this.RuleGroup.IsSelected = true;
            }
        }

        public void RemoveExecutionConditions()
        {
            this.RuleGroup = null;
            this.Model.RuleGroup = null;
        }

        protected override bool CanMoveDown()
        {
            return true;
        }

        protected override bool CanMoveUp()
        {
            return true;
        }

        private bool CanRemoveExecutionConditions()
        {
            return this.Model.RuleGroup != null;
        }

        private bool CanAddExecutionConditions()
        {
            return this.RuleGroup == null;
        }
    }
}
