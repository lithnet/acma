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
using Lithnet.Acma.TestEngine;

namespace Lithnet.Acma.Presentation
{
    public class NewObjectModificationViewModel : ViewModelBase
    {
        UnitTest parentTest;

        public NewObjectModificationViewModel(Window window, UnitTest parentTest)
            : base()
        {
            this.parentTest = parentTest;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.CreationObject = this.AllowedObjects.FirstOrDefault();
            this.ModificationType = this.AllowedModificationTypes.FirstOrDefault();
            this.StepName = "New step";
        }

        public string DisplayName
        {
            get
            {
                return "Modify an existing object";
            }
        }

        public IEnumerable<UnitTestStepObjectCreation> AllowedObjects
        {
            get
            {
                return this.parentTest.GetObjectCreationSteps();
            }
        }

        public ObjectModificationType ModificationType { get; set; }

        public IEnumerable<ObjectModificationType> AllowedModificationTypes
        {
            get
            {
                yield return ObjectModificationType.Update;
                yield return ObjectModificationType.Delete;
                yield return ObjectModificationType.Replace;
            }
        }

        public string StepName { get; set; }

        public UnitTestStepObjectCreation CreationObject { get; set; }

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
            else if (propertyName == "CreationObject")
            {
                if (this.CreationObject == null)
                {
                    this.AddError("CreationObject", "A source object must be selected");
                }
                else{
                    this.RemoveError("CreationObject");
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
