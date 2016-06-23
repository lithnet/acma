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
    public class NewObjectCreationViewModel : ViewModelBase
    {
        private AcmaSchemaObjectClass objectClass;

        public NewObjectCreationViewModel(Window window)
            : base()
        {
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.StepName = "Test object";
            this.ObjectClass = this.ObjectClasses.FirstOrDefault();
        }

        public string DisplayName
        {
            get
            {
                return "Create new object";
            }
        }

        public IEnumerable<AcmaSchemaObjectClass> ObjectClasses
        {
            get
            {
                return ActiveConfig.DB.ObjectClasses;
            }
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.objectClass;
            }
            set 
            {
                bool hasDefaultName = false;

                if (this.objectClass != null)
                {
                    if (this.StepName == this.objectClass.Name)
                    {
                        hasDefaultName = true;    
                    }
                }

                this.objectClass = value;

                if (this.objectClass  != null)
                {
                    if (hasDefaultName)
                    {
                        this.StepName = this.ObjectClass.Name;
                    }
                }
            }
        }

        public string StepName { get; set; }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "StepName")
            {
                if (string.IsNullOrWhiteSpace(this.StepName))
                {
                    this.AddError("StepName", "Step name cannot be blank");
                }
                else
                {
                    this.RemoveError("StepName");
                }
            }
            else if (propertyName == "ObjectClass")
            {
                if (this.ObjectClass == null)
                {
                    this.AddError("ObjectClass","An object class must be selected");
                }
                else{
                    this.RemoveError("ObjectClass");
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
