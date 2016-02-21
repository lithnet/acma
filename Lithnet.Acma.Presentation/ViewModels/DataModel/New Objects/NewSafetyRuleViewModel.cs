using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class NewSafetyRuleViewModel : ViewModelBase
    {
        private SafetyRulesViewModel parentVM;

        public NewSafetyRuleViewModel(Window window, SafetyRulesViewModel parentVM)
            : base()
        {
            this.parentVM = parentVM;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                return "Create new safety rule";
            }
        }

        public string AttributeName
        {
            get
            {
                return this.parentVM.Mapping.Attribute.Name;
            }
        }

        public bool NullAllowed { get; set; }

        public string Pattern { get; set; }

        public string Name { get; set; }

        private bool CanCreate()
        {
            return !this.HasErrors;
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case "Name":
                    if (string.IsNullOrWhiteSpace(this.Name))
                    {
                        this.AddError("Name", "A name must be provided");
                    }
                    else if (ActiveConfig.DB.HasSafetyRule(this.Name))
                    {
                        this.AddError("Name", "The safety rule name is already in use");
                    }
                    else
                    {
                        this.RemoveError("Name");
                    }

                    break;

                case "Pattern":
                    try
                    {
                        Regex regex = new Regex(this.Pattern);
                        this.RemoveError("Pattern"); ;
                    }
                    catch
                    {
                        this.AddError("Pattern", "The regular expression syntax is invalid");
                    }

                    break;
                default:
                    break;
            }
        }

        private void Create(Window window)
        {
            try
            {
                this.parentVM.AddSafetyRule(this.Name, this.Pattern, this.NullAllowed);
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the rule\n\n" + ex.Message);
            }
        }
    }
}
