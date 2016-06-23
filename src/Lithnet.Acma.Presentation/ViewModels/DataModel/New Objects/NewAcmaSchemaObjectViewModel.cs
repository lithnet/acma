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
    public class NewAcmaSchemaObjectViewModel : ViewModelBase
    {
        private AcmaSchemaObjectsViewModel parentVM;

        public NewAcmaSchemaObjectViewModel(Window window, AcmaSchemaObjectsViewModel parentVM)
            : base()
        {
            this.Name = null;
            this.parentVM = parentVM;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                return "Create New Object Class";
            }
        }

        public string Name { get; set; }

        public bool IsShadowClass { get; set; }

        public AcmaSchemaObjectClass ShadowParent { get; set; }

        public IEnumerable<AcmaSchemaObjectClass> AvailableShadowParents
        {
            get
            {
                return ActiveConfig.DB.ObjectClasses;
                //return ActiveConfig.DB.ObjectClasses.Where(t => !t.IsShadowObject);
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Name")
            {
                this.ValidatePropertyChangeName();
            }

            if (propertyName == "ShadowParent" ||
                propertyName == "IsShadowClass")
            {
                this.ValdiatePropertyChangeShadowParent();
            }
        }

        private bool CanCreate()
        {
            return !this.HasErrors;
        }

        private void Create(Window window)
        {
            try
            {
                this.parentVM.CreateObjectClass(this.Name, this.ShadowParent);
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the object class\n\n" + ex.Message);
            }
        }

        private void ValidatePropertyChangeName()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.AddError("Name", "A name must be provided");
                return;
            }

            if (ActiveConfig.DB.HasObjectClass(this.Name))
            {
                this.AddError("Name", "The specified object class already exists");
                return;
            }

            if (this.Name.Length > 50)
            {
                this.AddError("Name", "The class name must be less than 50 characters");
                return;
            }

            if (Regex.IsMatch(this.Name, @"[^a-zA-Z0-9]+"))
            {
                this.AddError("Name", "The class name can contain only letters and numbers");
                return;
            }

            this.RemoveError("Name");
        }

        private void ValdiatePropertyChangeShadowParent()
        {
            if (this.IsShadowClass)
            {
                if (this.ShadowParent == null)
                {
                    this.AddError("ShadowParent", "A parent class must be selected");
                    return;
                }

                //if (this.ShadowParent.IsShadowObject)
                //{
                //    this.AddError("ShadowParent", "A parent class cannot be another shadow class");
                //    return;
                //}
            }

            this.RemoveError("ShadowParent");
        }
    }
}
