using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Lithnet.Acma.Presentation
{
    public class DeclarativeValueConstructorViewModel : AttributeConstructorViewModel
    {
        private DeclarativeValueConstructor typedModel;

        private ValueDeclarationsViewModel valueDeclarations;

        private RuleGroupViewModel presenceConditions;

        public DeclarativeValueConstructorViewModel(DeclarativeValueConstructor model)
            : base(model)
        {
            this.typedModel = model;

            if (this.typedModel.ValueDeclarations == null)
            {
                this.typedModel.ValueDeclarations = new List<ValueDeclaration>();
            }

            this.ValueDeclarations = new ValueDeclarationsViewModel(this.typedModel.ValueDeclarations, model.ObjectClass);

            if (this.ValueDeclarations.Count == 0)
            {
                this.ValueDeclarations.Add(new ValueDeclaration());
            }

            this.ValueDeclarations.CollectionChanged += ValueDeclarations_CollectionChanged;
            this.ValueDeclarationBindingList = this.ValueDeclarations.GetNewBindingList();

            if (this.typedModel.ModificationType == AcmaAttributeModificationType.Conditional)
            {
                if (this.typedModel.PresenceConditions == null)
                {
                    this.typedModel.PresenceConditions = new RuleGroup();
                }

                this.PresenceConditions = new RuleGroupViewModel(this.typedModel.PresenceConditions, true, "Presence conditions");
            }


            this.ValidatePropertyChange("Attribute");
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                if (this.RuleGroup != null)
                {
                    yield return this.RuleGroup;
                }

                if (this.PresenceConditions != null)
                {
                    yield return this.PresenceConditions;
                }
            }
        }

        public AcmaAttributeModificationType ModificationType
        {
            get
            {
                return this.typedModel.ModificationType;

            }
            set
            {
                this.typedModel.ModificationType = value;
            }
        }

        public bool IsConditionalPresenceEnabled
        {
            get
            {
                return this.ModificationType == AcmaAttributeModificationType.Conditional;
            }
        }

        public RuleGroupViewModel PresenceConditions
        {
            get
            {
                return this.presenceConditions;
            }
            set
            {
                if (this.presenceConditions != null)
                {
                    this.UnregisterChildViewModel(this.presenceConditions);
                }

                this.presenceConditions = value;

                if (this.presenceConditions != null)
                {
                    this.RegisterChildViewModel(this.presenceConditions);
                    this.presenceConditions.ParentConstructor = this;
                }
            }
        }

        public ValueDeclarationsViewModel ValueDeclarations
        {
            get
            {
                return this.valueDeclarations;
            }
            set
            {
                if (this.valueDeclarations != null)
                {
                    this.UnregisterChildViewModel(this.valueDeclarations);
                }

                this.valueDeclarations = value;

                if (this.valueDeclarations != null)
                {
                    this.RegisterChildViewModel(this.valueDeclarations);
                }
            }
        }

        public ObservableCollection<ValueDeclarationViewModel> ValueDeclarationBindingList { get; private set; }

        public bool CanEditDeclarations
        {
            get
            {
                return this.Attribute != null;
            }
        }

        public bool CanAddNewValueDeclarations
        {
            get
            {
                if (this.Attribute != null)
                {
                    if (this.Attribute.IsMultivalued)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool CanDeleteValueDeclarations { get; set; }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Attribute")
            {
                this.RaisePropertyChanged("CanAddNewValueDeclarations");
                this.RaisePropertyChanged("CanEditDeclarations");

                this.ValidateDeclarationCount();
            }
            else if (propertyName == "ModificationType")
            {
                if (this.IsConditionalPresenceEnabled)
                {
                    if (this.PresenceConditions == null)
                    {
                        this.typedModel.PresenceConditions = new RuleGroup();
                        this.PresenceConditions = new RuleGroupViewModel(this.typedModel.PresenceConditions, true, "Presence conditions");
                        this.IsExpanded = true;
                        this.PresenceConditions.IsExpanded = true;
                    }
                }
                else
                {
                    if (this.PresenceConditions != null)
                    {
                        this.PresenceConditions = null;
                        this.typedModel.PresenceConditions = null;
                    }
                }
            }
        }

        private void ValidateDeclarationCount()
        {
            if (this.Attribute != null)
            {
                if (!this.Attribute.IsMultivalued && this.ValueDeclarations.Count > 1)
                {
                    this.AddError("ValueDeclarations", "Only a single declaration is allowed for a single-valued attribute");
                }
                else
                {
                    this.RemoveError("ValueDeclarations");
                }
            }
        }

        private void ValueDeclarations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CanDeleteValueDeclarations = this.ValueDeclarations.Count > 1;
            this.ValidateDeclarationCount();
        }
    }
}
