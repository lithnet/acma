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
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class NewAcmaSchemaAttributeViewModel : ViewModelBase
    {
        private ExtendedAttributeType type;

        private AcmaSchemaAttributesViewModel attributesVM;

        public NewAcmaSchemaAttributeViewModel(NewAttributeWindow window, AcmaSchemaAttributesViewModel attributesVM)
            : base()
        {
            this.attributesVM = attributesVM;
            this.Name = null;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.Commands.AddItem("CreateAndAddNew", t => this.CreateAndAddNew(window), t => this.CanCreate());

            this.Type = ExtendedAttributeType.String;
            this.IsIndexable = true;
            this.IsIndexableAllowed = true;
            this.IsMultivaluedAllowed = true;
            this.ConfigureForAttributeType();
        }
        
        public string DisplayName
        {
            get
            {
                return "Create New Attribute";
            }
        }


        public string Name { get; set; }

        public bool IsMultivalued { get; set; }

        public bool IsMultivaluedAllowed { get; set; }

        public bool IsIndexableAllowed { get; set; }

        public bool IsIndexedAllowed { get; set; }

        public bool IsIndexable { get; set; }

        public bool IsIndexed { get; set; }

        public AcmaAttributeOperation Operation { get; set; }

        public IEnumerable<EnumExtension.EnumMember> AllowedDataTypes
        {
            get
            {
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.String.GetEnumDescription(), Value = ExtendedAttributeType.String };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Integer.GetEnumDescription(), Value = ExtendedAttributeType.Integer };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Reference.GetEnumDescription(), Value = ExtendedAttributeType.Reference };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Boolean.GetEnumDescription(), Value = ExtendedAttributeType.Boolean };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Binary.GetEnumDescription(), Value = ExtendedAttributeType.Binary };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.DateTime.GetEnumDescription(), Value = ExtendedAttributeType.DateTime };
            }
        }

        public ExtendedAttributeType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                this.ConfigureForAttributeType();
            }
        }

        private void ConfigureForAttributeType()
        {
            switch (this.type)
            {
                case ExtendedAttributeType.Binary:
                    this.IsIndexableAllowed = true;
                    this.IsMultivaluedAllowed = true;
                    this.IsIndexedAllowed = true;
                    this.IsMultivalued = false;
                    this.IsIndexable = true;
                    this.IsIndexed = false;
                    break;

                case ExtendedAttributeType.Boolean:
                    this.IsIndexableAllowed = false;
                    this.IsMultivaluedAllowed = false;
                    this.IsIndexedAllowed = false;
                    this.IsMultivalued = false;
                    this.IsIndexable = false;
                    this.IsIndexed = false;
                    break;

                case ExtendedAttributeType.Integer:
                    this.IsIndexableAllowed = false;
                    this.IsIndexedAllowed = true;
                    this.IsMultivalued = false;
                    this.IsMultivaluedAllowed = true;
                    this.IsIndexable = true;
                    this.IsIndexed = false;
                    break;

                case ExtendedAttributeType.DateTime:
                    this.IsIndexableAllowed = false;
                    this.IsIndexedAllowed = true;
                    this.IsMultivalued = false;
                    this.IsMultivaluedAllowed = true;
                    this.IsIndexable = true;
                    this.IsIndexed = false;
                    break;

                case ExtendedAttributeType.Reference:
                    this.IsIndexableAllowed = false;
                    this.IsIndexedAllowed = false;
                    this.IsIndexed = true;
                    this.IsIndexable = true;
                    this.IsMultivaluedAllowed = true;
                    this.IsMultivalued = false;
                    break;

                case ExtendedAttributeType.String:
                    this.IsIndexableAllowed = true;
                    this.IsIndexedAllowed = true;
                    this.IsMultivaluedAllowed = true;
                    this.IsMultivalued = false;
                    this.IsIndexed = false;
                    this.IsIndexable = true;
                    break;

                case ExtendedAttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Name")
            {
                this.ValidatePropertyChangeName();
            }
            else
            {
                this.ValidateProperties();
            }

        }

        private bool CanCreate()
        {
            return !this.HasErrors;
        }

        private void Create(NewAttributeWindow window, bool close = true)
        {
            try
            {
                this.attributesVM.CreateAttribute(this.Name, this.Type, this.IsMultivalued, this.Operation, this.IsIndexable, this.IsIndexed);

                if (close)
                {
                    window.Close();
                }
                else
                {
                    this.Name = null;
                    this.Type = ExtendedAttributeType.String;
                    this.IsIndexed = false;
                    this.IsIndexable = true;
                    this.IsMultivalued = false;
                    window.AttributeName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the attribute\n\n" + ex.Message);
            }
        }

        private void CreateAndAddNew(NewAttributeWindow window)
        {
            this.Create(window, false);
        }

        private void ValidatePropertyChangeName()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.AddError("Name", "A name must be provided");
                return;
            }

            if (ActiveConfig.DB.HasAttribute(this.Name))
            {
                this.AddError("Name", "The specified attribute already exists");
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
            switch (this.Type)
            {
                case ExtendedAttributeType.Binary:

                    break;
                case ExtendedAttributeType.Boolean:
                    break;
                case ExtendedAttributeType.Integer:
                    break;
                case ExtendedAttributeType.Reference:
                    break;
                case ExtendedAttributeType.String:
                    break;
                case ExtendedAttributeType.Undefined:
                    break;
                default:
                    break;
            }
        }
    }
}
