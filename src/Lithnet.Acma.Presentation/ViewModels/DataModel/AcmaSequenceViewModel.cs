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
    public class AcmaSequenceViewModel : ViewModelBase<AcmaSequence>
    {
        private AcmaSequence model;

        private AcmaSequencesViewModel parentVM;

        public AcmaSequenceViewModel(AcmaSequence model, AcmaSequencesViewModel parentVM)
            : base(model)
        {
            this.Commands.AddItem("DeleteSequence", t => this.Delete());
            this.Commands.AddItem("Edit", t => this.Edit(), t => this.CanEdit());
            this.Commands.AddItem("Cancel", t => this.Cancel());
            this.model = model;
            this.parentVM = parentVM;
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

        public bool IsCycleEnabled
        {
            get
            {
                return this.model.IsCycleEnabled.HasValue ? this.model.IsCycleEnabled.Value : false;
            }
            set
            {
                this.model.IsCycleEnabled = value;
            }
        }

        public string StartValue
        {
            get
            {
                return this.model.StartValue.ToString();
            }
            set
            {
                long outValue;

                if (long.TryParse(value, out outValue))
                {
                    this.model.StartValue = outValue;
                    this.RemoveError("StartValue");
                }
                else
                {
                    this.AddError("StartValue", "Only numbers are allowed in this field");
                }
            }
        }

        public string MinValue
        {
            get
            {
                return this.model.MinValue.ToString();
            }
            set
            {
                long outValue;

                if (string.IsNullOrWhiteSpace(value))
                {
                    this.model.MinValue = null;
                    this.RemoveError("MinValue");
                }
                else  if (long.TryParse(value, out outValue))
                {
                    this.model.MinValue = outValue;
                    this.RemoveError("MinValue");
                }
                else
                {
                    this.AddError("MinValue", "Only numbers are allowed in this field");
                }
            }
        }

        public string MaxValue
        {
            get
            {
                return this.model.MaxValue.ToString();
            }
            set
            {
                long outValue;

                if (string.IsNullOrWhiteSpace(value))
                {
                    this.model.MaxValue = null;
                    this.RemoveError("MaxValue");
                }
                else if (long.TryParse(value, out outValue))
                {
                    this.model.MaxValue = outValue;
                    this.RemoveError("MaxValue");
                }
                else
                {
                    this.AddError("MaxValue", "Only numbers are allowed in this field");
                }
            }
        }

        public long CurrentValue
        {
            get
            {
                return this.model.CurrentValue;
            }

        }

        public string Increment
        {
            get
            {
                return this.model.Increment.ToString();
            }
            set
            {
                long outValue;

                if (long.TryParse(value, out outValue))
                {
                    this.model.Increment = outValue;
                    this.RemoveError("Increment");
                }
                else
                {
                    this.AddError("Increment", "Only numbers are allowed in this field");
                }
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
            ActiveConfig.DB.UpdateSequence(this.Model);
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
            this.MinValue = this.model.MinValue.ToString();
            this.MaxValue = this.model.MaxValue.ToString();
            this.StartValue = this.model.StartValue.ToString();
            this.Increment = this.model.Increment.ToString();

            this.RaisePropertyChanged("MinValue");
            this.RaisePropertyChanged("MaxValue");
            this.RaisePropertyChanged("StartValue");
            this.RaisePropertyChanged("CurrentValue");
            this.RaisePropertyChanged("Name");
            this.RaisePropertyChanged("Increment");
            this.RaisePropertyChanged("IsCycleEnabled");
        }

        private void Delete()
        {
            try
            {
                if (MessageBox.Show("Are you are you want to delete this sequence?", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    this.parentVM.DeleteSequence(this.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the sequence\n\n" + ex.Message);
            }
        }
    }
}
