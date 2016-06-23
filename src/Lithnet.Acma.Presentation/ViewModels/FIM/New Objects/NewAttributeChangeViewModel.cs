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
    public class NewAttributeChangeViewModel : ViewModelBase
    {
        private static AcmaSchemaAttribute lastSelectedAttribute;

        public NewAttributeChangeViewModel(Window window, IEnumerable<AcmaSchemaAttribute> allowedAttributes, IEnumerable<AttributeModificationType> allowedModificationTypes)
            : base()
        {
            this.AllowedAttributes = allowedAttributes.OrderBy(t => t.Name);
            this.AllowedModificationTypes = allowedModificationTypes;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.ModificationType = this.AllowedModificationTypes.FirstOrDefault();
            
            if (NewAttributeChangeViewModel.lastSelectedAttribute != null)
            {
                if (this.AllowedAttributes.Contains(NewAttributeChangeViewModel.lastSelectedAttribute))
                {
                    this.Attribute = NewAttributeChangeViewModel.lastSelectedAttribute;
                }
            }

            if (this.Attribute == null)
            {
                this.Attribute = this.AllowedAttributes.FirstOrDefault();
            }
        }

        public string DisplayName
        {
            get
            {
                return "Create attribute change";
            }
        }

        public AttributeModificationType ModificationType { get; set; }

        public AcmaSchemaAttribute Attribute { get; set; }

        public IEnumerable<AcmaSchemaAttribute> AllowedAttributes { get; set; }

        public IEnumerable<AttributeModificationType> AllowedModificationTypes { get; set; }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Attribute")
            {
                if (this.Attribute == null)
                {
                    this.AddError("Attribute", "Attribute cannot be blank");
                }
                else
                {
                    NewAttributeChangeViewModel.lastSelectedAttribute = this.Attribute;
                    this.RemoveError("Attribute");
                }
            }
            else if (propertyName == "ModificationType")
            {
                if (this.AllowedModificationTypes.Contains(this.ModificationType))
                {
                    this.RemoveError("ModificationType");
                }
                else
                {
                    this.AddError("ModificationType", "Invalid modification type");
                }
            }
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
    }
}
