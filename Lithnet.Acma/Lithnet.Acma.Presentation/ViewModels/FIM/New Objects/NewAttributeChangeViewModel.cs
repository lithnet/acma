using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
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
        public NewAttributeChangeViewModel(Window window, IEnumerable<AcmaSchemaAttribute> allowedAttributes, IEnumerable<AttributeModificationType> allowedModificationTypes)
            : base()
        {
            this.AllowedAttributes = allowedAttributes.OrderBy(t => t.Name);
            this.AllowedModificationTypes = allowedModificationTypes;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.ModificationType = this.AllowedModificationTypes.FirstOrDefault();
            this.Attribute = this.AllowedAttributes.FirstOrDefault();
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
