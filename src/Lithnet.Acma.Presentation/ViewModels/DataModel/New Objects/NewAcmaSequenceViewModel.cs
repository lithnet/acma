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
    public class NewAcmaSequenceViewModel : ViewModelBase
    {
        public NewAcmaSequenceViewModel(Window window)
            : base()
        {
            this.Name = null;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.StartValue = 1;
            this.MinValue = null;
            this.MaxValue = null;
            this.Increment = 1;
            this.IsCycleEnabled = false;
            this.SetDefaultName();
        }

        public string DisplayName
        {
            get
            {
                return "Create New Sequence";
            }
        }

        public string Name { get; set; }

        public long StartValue { get; set; }

        public long? MinValue { get; set; }

        public long? MaxValue { get; set; }

        public long Increment { get; set; }

        public bool IsCycleEnabled { get; set; }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Name")
            {
                this.ValidatePropertyChangeName();
            }
            else if (propertyName == "MinValue" || propertyName == "MaxValue" || propertyName == "StartValue")
            {
                this.ValidateProperties();
            }
            
        }

        private void SetDefaultName()
        {
            int count = 1;
            string startName = "NewSequence";
            string createdName = startName;

            while (ActiveConfig.DB.HasSequence(createdName))
            {
                createdName = startName + count;
                count++;
            }

            this.Name = createdName;
        }

        private bool CanCreate()
        {
            return !this.HasErrors;
        }

        private void Create(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }

        private void ValidatePropertyChangeName()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.AddError("Name", "A name must be provided");
                return;
            }

            if (ActiveConfig.DB.HasSequence(this.Name))
            {
                this.AddError("Name", "The specified sequence already exists");
                return;
            }

            if (this.Name.Length > 50)
            {
                this.AddError("Name", "The name must be less than 50 characters");
                return;
            }

            if (Regex.IsMatch(this.Name, @"[^a-zA-Z0-9]+"))
            {
                this.AddError("Name", "The name can contain only letters and numbers");
                return;
            }

            this.RemoveError("Name");
        }

        private void ValidateProperties()
        {
            if (this.MinValue.HasValue && !this.MaxValue.HasValue)
            {
                this.AddError("MaxValue", "A maximum value must be provided if a minimum value is provided");
            }
            else if (this.MaxValue.HasValue && !this.MinValue.HasValue)
            {
                this.AddError("MinValue", "A minimum value must be provided if a maximum value is provided");
            }
            else if (this.MinValue.HasValue && this.MaxValue.HasValue)
            {
                if (this.MaxValue.Value <= this.MinValue.Value)
                {
                    this.AddError("MaxValue", "The maximum value must be greater than the minimum value");
                }
                else if (this.StartValue < this.MinValue.Value)
                {
                    this.AddError("StartValue", "The start value must be greater than the minimum value");
                }
                else
                {
                    this.RemoveError("MaxValue");
                    this.RemoveError("MinValue");
                    this.RemoveError("StartValue");
                }
            }
            else
            {
                this.RemoveError("MinValue");
                this.RemoveError("MaxValue");
                this.RemoveError("StartValue");
            }

            

        }
    }
}
