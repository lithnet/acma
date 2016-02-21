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
using Lithnet.Acma.TestEngine;

namespace Lithnet.Acma.Presentation
{
    public class NewValueChangeViewModel : ViewModelBase
    {
        private ExtendedAttributeType type;

        private ValueChangeViewModel existingValueChange;

        private Guid referenceValue;

        public NewValueChangeViewModel(
            Window window, 
            IEnumerable<UnitTestStepObjectCreation> allowedReferenceObjects, 
            ExtendedAttributeType dataType,
            ValueModificationType modificationType,
            ValueChangeViewModel existingValueChange = null)
            : base()
        {
            this.IgnoreOwnPropertyChanges = true;
            this.AllowedObjects = allowedReferenceObjects;
            this.existingValueChange = existingValueChange;
            this.ModificationType = modificationType;
            this.Type = dataType;

            if (this.existingValueChange != null)
            {
                this.ModificationType = this.existingValueChange.ModificationType;

                if (this.Type == ExtendedAttributeType.Reference)
                {
                    if (!string.IsNullOrWhiteSpace(this.existingValueChange.Value))
                    {
                        object modelValue = this.existingValueChange.ModelValue;
                        Guid modelValueGuid = Lithnet.MetadirectoryServices.TypeConverter.ConvertData<Guid>(modelValue);
                        this.ReferenceValue = modelValueGuid;
                    }
                    else 
                    {
                        this.ReferenceValue = Guid.Empty;
                    }
                }
                else
                {
                    this.Value = this.existingValueChange.Value;
                }
            }
            else
            {
                if (this.type == ExtendedAttributeType.Reference)
                {
                    if (this.AllowedObjects.Any())
                    {
                        this.ReferenceValue = this.AllowedObjects.First().ObjectId;
                    }
                }
            }
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                if (this.existingValueChange == null)
                {
                    return "Create value change";
                }
                else
                {
                    return "Edit value change";
                }
            }
        }

        public ValueModificationType ModificationType { get; set; }

        public ExtendedAttributeType Type
        {
            get
            {
                return this.type;
            }
            private set
            {
                this.type = value;

                if (this.IsReferenceAttribute && this.AllowedObjects.Count() > 0)
                {
                    this.Value = this.AllowedObjects.First().ObjectId.ToString();
                }
            }
        }

        public string Value { get; set; }

        public Guid ReferenceValue
        {
            get
            {
                return this.referenceValue;
            }
            set
            {
                this.referenceValue = value;
                this.Value = this.referenceValue.ToSmartString();
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            if (propertyName == "Value")
            {
                if (string.IsNullOrWhiteSpace(this.Value))
                {
                    this.AddError("Value", "Value cannot be blank");
                }
                else
                {
                    object obj;

                    if (!Lithnet.MetadirectoryServices.TypeConverter.TryConvertData(this.Value, this.Type, out obj))
                    {
                        this.AddError("Value", string.Format("The specified value could not be converted to a {0} data type", this.Type.ToSmartString()));
                    }
                    else
                    {
                        this.RemoveError("Value");
                    }
                }
            }
            else if (propertyName == "ReferenceValue")
            {
                if (this.ReferenceValue == Guid.Empty)
                {
                    this.AddError("ReferenceValue", "An object must be selected");

                }
                else if (!this.AllowedObjects.Any(t => t.ObjectId == this.ReferenceValue))
                {
                    this.AddError("ReferenceValue", "An invalid object is selected");
                }
                else
                {
                    this.RemoveError("ReferenceValue");
                }
            }
        }

        public bool IsReferenceAttribute
        {
            get
            {
                return this.Type == ExtendedAttributeType.Reference;
            }
        }

        public bool IsNonReferenceAttribute
        {
            get
            {
                return this.Type != ExtendedAttributeType.Reference;
            }
        }
        public IEnumerable<UnitTestStepObjectCreation> AllowedObjects { get; private set; }

        private bool CanCreate()
        {
            return !this.HasErrors;
        }

        private void Create(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }
    }
}
