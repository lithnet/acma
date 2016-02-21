using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;

namespace Lithnet.Acma.Presentation
{
    public class SafetyRuleViewModel : ViewModelBase<SafetyRule>
    {
        private SafetyRule model;

        public SafetyRuleViewModel(SafetyRule model)
            : base(model)
        {
            this.Commands.AddItem("Delete", t => this.Delete());
            this.Commands.AddItem("Edit", t => this.Edit());
            this.Commands.AddItem("Cancel", t => this.Cancel());
            this.model = model;
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0}", this.Name);
            }
        }

        public string Name
        {
            get
            {
                return this.model.Name;
            }
            set
            {
                this.model.Name = value;
            }
        }

        public string Pattern
        {
            get
            {
                return this.model.Pattern;
            }
            set
            {
                this.model.Pattern = value;
            }
        }

        public bool NullAllowed
        {
            get
            {
                return this.model.NullAllowed;
            }
            set
            {
                this.model.NullAllowed = value;
            }
        }

        public AcmaSchemaAttribute Attribute
        {
            get
            {
                return this.model.AcmaSchemaMapping.Attribute;
            }
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.model.AcmaSchemaMapping.ObjectClass;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return !this.IsEditing;
            }
        }

        public bool IsEditing { get; set; }

        private void Edit()
        {
            if (this.IsEditing)
            {
                this.IsEditing = false;
                this.Save();
            }
            else
            {
                this.IsEditing = true;
            }
        }

        private bool CanEdit()
        {
            if (this.IsEditing)
            {
                return !this.HasErrors;
            }
            else
            {
                return true;
            }
        }

        public string EditButtonText
        {
            get
            {
                if (this.IsEditing)
                {
                    return "Save";
                }
                else
                {
                    return "Edit";
                }
            }
        }

        private void Save()
        {
            ActiveConfig.DB.UpdateSafetyRule(this.Model);
            ActiveConfig.DB.RefreshEntity(this.Model);
            this.RefreshViewModelProperties();
        }

        private void Cancel()
        {
            this.IsEditing = false;
            ActiveConfig.DB.RefreshEntity(this.Model);
            this.RefreshViewModelProperties();
        }

        private void RefreshViewModelProperties()
        {
            this.Name = this.model.Name;
            this.Pattern = this.model.Pattern;
            this.NullAllowed = this.model.NullAllowed;

            this.RaisePropertyChanged("Name");
            this.RaisePropertyChanged("Pattern");
            this.RaisePropertyChanged("NullAllowed");
        }

        private void Delete()
        {
            try
            {
                if (MessageBox.Show("Are you are you want to delete this rule?", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    ActiveConfig.DB.DeleteSafetyRule(this.model);
                    this.ParentCollection.Remove(this.model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the rule\n\n" + ex.Message);
            }
        }
    }
}
