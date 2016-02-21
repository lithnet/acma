using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class RuleGroupViewModel : RuleObjectViewModel
    {
        private RuleGroup typedModel;

        private ListViewModel<RuleObjectViewModel, RuleObject> rules;

        private string displayName;

        public RuleGroupViewModel(RuleGroup model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;

            this.Rules = new ListViewModel<RuleObjectViewModel, RuleObject>((IList)model.Items, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");

            this.Commands.AddItem("DeleteRuleGroup", t => this.DeleteRuleGroup());
            this.Commands.AddItem("AddRuleGroup", t => this.AddRuleGroup());
            this.Commands.AddItem("AddObjectChangeRule", t => this.AddObjectChangeRule(), t => this.CanAddEventBasedRules());
            this.Commands.AddItem("AddAttributeChangeRule", t => this.AddAttributeChangeRule(), t => this.CanAddEventBasedRules());
            this.Commands.AddItem("AddAttributePresenceRule", t => this.AddAttributePresenceRule());
            this.Commands.AddItem("AddEventRule", t => this.AddEventRule(), t => this.CanAddEventBasedRules());
            this.Commands.AddItem("AddValueComparisonRule", t => this.AddValueComparisonRule());
            this.Commands.AddItem("AddAdvancedComparisonRule", t => this.AddAdvancedComparisonRule());
            this.Commands.AddItem("Paste", t => this.Rules.Paste(), t => this.Rules.CanPaste());

            this.Rules.PasteableTypes.Add(typeof(RuleGroup));
            this.Rules.PasteableTypes.Add(typeof(AttributeChangeRule));
            this.Rules.PasteableTypes.Add(typeof(AttributePresenceRule));
            this.Rules.PasteableTypes.Add(typeof(EventRule));
            this.Rules.PasteableTypes.Add(typeof(ObjectChangeRule));
            this.Rules.PasteableTypes.Add(typeof(ValueComparisonRule));
            this.Rules.PasteableTypes.Add(typeof(AdvancedComparisonRule));

            this.EnableCutCopy();
        }

        public RuleGroupViewModel(RuleGroup model, bool canUseProposedValues, string displayName)
            : this(model, canUseProposedValues)
        {
            this.displayName = displayName;
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                return this.Rules;
            }
        }

        public ListViewModel<RuleObjectViewModel, RuleObject> Rules
        {
            get
            {
                return this.rules;
            }
            set
            {
                if (this.rules != null)
                {
                    this.UnregisterChildViewModel(this.rules);
                }

                this.rules = value;
                this.RegisterChildViewModel(this.rules);
            }
        }

        public bool BlockEventRules
        {
            get
            {
                return this.typedModel.BlockEventRules;
            }
            set
            {
                this.typedModel.BlockEventRules = value;
            }
        }

        public ExecutableConstructorObjectViewModel ParentConstructor { get; set; }

        private string GetDisplayName()
        {
            if (string.IsNullOrWhiteSpace(this.displayName))
            {
                return string.Format("Rule group ({0})", this.Operator.ToSmartString().ToLower());
            }
            else
            {
                return string.Format("{0} ({1})", this.displayName, this.Operator.ToSmartString().ToLower());
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
        
        public GroupOperator Operator
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

        private void DeleteRuleGroup()
        {
            try
            {
                if (this.Rules.Count > 0)
                {
                    if (MessageBox.Show("Are you are you want to delete this group?", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                if (this.ParentConstructor != null)
                {
                    this.ParentConstructor.RemoveExecutionConditions();
                }
                else
                {
                    if (this.ParentCollection != null)
                    {
                        this.ParentCollection.Remove(this.Model);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the group\n\n" + ex.Message);
            }
        }

        private RuleObjectViewModel ViewModelResolver(RuleObject model)
        {
            if (model is RuleGroup)
            {
                return new RuleGroupViewModel(model as RuleGroup, this.CanUseProposedValues);
            }
            else if (model is AttributeChangeRule)
            {
                return new AttributeChangeRuleViewModel(model as AttributeChangeRule, this.CanUseProposedValues);
            }
            else if (model is AttributePresenceRule)
            {
                return new AttributePresenceRuleViewModel(model as AttributePresenceRule, this.CanUseProposedValues);
            }
            else if (model is ValueComparisonRule)
            {
                return new ValueComparisonRuleViewModel(model as ValueComparisonRule, this.CanUseProposedValues);
            }
            else if (model is AdvancedComparisonRule)
            {
                return new AdvancedComparisonRuleViewModel(model as AdvancedComparisonRule, this.CanUseProposedValues);
            }
            else if (model is ObjectChangeRule)
            {
                return new ObjectChangeRuleViewModel(model as ObjectChangeRule, this.CanUseProposedValues);
            }
            else if (model is EventRule)
            {
                return new EventRuleViewModel(model as EventRule, this.CanUseProposedValues);
            }
            else
            {
                throw new ArgumentException("The object type is unknown", "model");
            }
        }

        public void AddRuleGroup()
        {
            this.IsExpanded = true;
            this.Rules.Add(new RuleGroup(), true);
        }

        public void AddAttributeChangeRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new AttributeChangeRule(), true);
        }

        public void AddAttributePresenceRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new AttributePresenceRule(), true);
        }

        public void AddValueComparisonRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new ValueComparisonRule(), true);
        }

        public void AddAdvancedComparisonRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new AdvancedComparisonRule(), true);
        }

        public void AddObjectChangeRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new ObjectChangeRule(), true);
        }

        public void AddEventRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new EventRule(), true);
        }

        public bool CanAddEventBasedRules()
        {
            return !this.BlockEventRules;
        }
    }
}
