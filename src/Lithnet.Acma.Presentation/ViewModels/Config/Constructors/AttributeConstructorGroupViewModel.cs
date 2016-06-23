using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lithnet.Acma.Presentation
{
    public class AttributeConstructorGroupViewModel : ExecutableConstructorObjectViewModel
    {
        private AttributeConstructorGroup typedModel;

        private ConstructorsViewModel constructors;

        public AttributeConstructorGroupViewModel(AttributeConstructorGroup model)
            : base(model)
        {
            this.typedModel = model;
            this.Constructors = new ConstructorsViewModel(this.typedModel.Constructors);
            this.Constructors.Parent = this;
            this.Commands.AddItem("AddConstructorGroup", t => this.Constructors.AddConstructorGroup());
            this.Commands.AddItem("DeleteConstructorGroup", t => this.DeleteConstructorGroup());
            this.Commands.AddItem("AddDVConstructor", t => this.Constructors.AddDVConstructor());
            this.Commands.AddItem("AddSIAConstructor", t => this.Constructors.AddSIAConstructor());
            this.Commands.AddItem("AddVDConstructor", t => this.Constructors.AddVDConstructor());
            this.Commands.AddItem("AddRLConstructor", t => this.Constructors.AddRLConstructor());
            this.Commands.AddItem("AddUVConstructor", t => this.Constructors.AddUVConstructor());
            this.Commands.AddItem("Paste", t => this.Constructors.Paste(), t => this.Constructors.CanPaste());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.EnableCutCopy();
        }

        public override string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", this.Id, this.ExecutionRule.GetEnumDescription().ToLower());
            }
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                if (this.RuleGroup != null)
                {
                    yield return this.RuleGroup;
                }

                foreach (ViewModelBase vm in this.Constructors)
                {
                    yield return vm;
                }
            }
        }

        public GroupExecutionRule ExecutionRule
        {
            get
            {
                return this.typedModel.ExecutionRule;
            }
            set
            {
                this.typedModel.ExecutionRule = value;
            }
        }

        public ConstructorsViewModel Constructors
        {
            get
            {
                return this.constructors;
            }
            set
            {
                if (this.constructors != null)
                {
                    this.UnregisterChildViewModel(this.constructors);
                }

                this.constructors = value;

                this.RegisterChildViewModel(this.constructors);
            }
        }

        private void DeleteConstructorGroup()
        {
            try
            {
                if (this.Constructors.Count > 0)
                {
                    if (MessageBox.Show("Are you are you want to delete this group?", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                if (this.ParentCollection != null)
                {
                    this.ParentCollection.Remove(this.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the group\n\n" + ex.Message);
            }
        }
    }
}
