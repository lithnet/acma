using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Common.Presentation;
using Lithnet.Fim.Core;
using System.Reflection;
using System.Windows;
using System.Text.RegularExpressions;
using Lithnet.Fim.Transforms;

namespace Lithnet.Fim.UniversalMARE.Presentation
{
    public class NewAliasViewModel : ViewModelBase<FlowRuleAlias>
    {
        private FlowRuleAliasCollectionViewModel collectionViewModel;

        public NewAliasViewModel(FlowRuleAliasCollectionViewModel aliases, Window window)
            : base()
        {
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.collectionViewModel = aliases;
        }

        private bool CanCreate()
        {
            if (string.IsNullOrWhiteSpace(this.Alias))
            {
                return false;
            }

            this.ValidatePropertyChange("Alias");
            this.ValidatePropertyChange("FlowRuleDefinition");
            return !this.HasErrors;
        }

        private void Create(Window window)
        {
            FlowRuleAlias newItem = new FlowRuleAlias();
            newItem.Alias = this.Alias;
            newItem.FlowRuleDefinition = this.FlowRuleDefinition;
            this.collectionViewModel.Add(newItem);
            this.collectionViewModel.IsExpanded = true;

            window.Close();
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case "Alias":
                    if (string.IsNullOrWhiteSpace(this.Alias))
                    {
                        this.AddError("Alias", "An alias must be specified");
                    }
                    else
                    {
                        if (Regex.IsMatch(this.Alias, @"[^a-zA-Z0-9]+"))
                        {
                            this.AddError("Alias", "The ID must contain only letters, numbers, hyphen, and underscores");
                        }
                        else
                        {
                            this.RemoveError("Alias");
                        }
                    }

                    break;

                case "FlowRuleDefinition":
                    if (string.IsNullOrWhiteSpace(this.FlowRuleDefinition))
                    {
                        this.AddError("FlowRuleDefinition", "A flow rule definition must be specified");
                    }
                    else
                    {
                        this.RemoveError("FlowRuleDefinition");
                    }
                    break;

                default:
                    break;
            }
        }

        public string Alias { get; set; }

        public string FlowRuleDefinition { get; set; }
    }
}