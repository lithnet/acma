using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Lithnet.Acma.DataModel;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.Acma.TestEngine;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestViewModel : UnitTestObjectViewModel
    {
        private UnitTestStepsViewModel unitTestStepsViewModel;

        private UnitTest typedModel;

        public UnitTestViewModel(UnitTest model)
            : base(model)
        {
            this.typedModel = model;
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.Steps = new UnitTestStepsViewModel(this.typedModel.Steps, this.typedModel);
            this.Commands.AddItem("AddUnitTestObjectEvaluation", t => this.Steps.AddUnitTestObjectEvaluation(), t => this.Steps.CanAddUnitTestObjectEvaluation());
            this.Commands.AddItem("AddUnitTestObjectModification", t => this.Steps.AddUnitTestObjectModification(), t => this.Steps.CanAddUnitTestObjectModification());
            this.Commands.AddItem("AddUnitTestObjectCreation", t => this.Steps.AddUnitTestObjectCreation());
            this.Commands.AddItem("Paste", t => this.Steps.Paste(), t => this.Steps.CanPaste());
            this.Commands.AddItem("Delete", t => this.Delete());
            this.EnableCutCopy();
        }
        
        public UnitTestStepsViewModel Steps
        {
            get
            {
                return this.unitTestStepsViewModel;
            }
            set
            {
                if (this.unitTestStepsViewModel != null)
                {
                    this.UnregisterChildViewModel(this.unitTestStepsViewModel);
                }

                this.unitTestStepsViewModel = value;

                this.RegisterChildViewModel(this.unitTestStepsViewModel);
            }
        }

        public ObservableCollection<string> ExpectedConstructors
        {
            get
            {
                return this.typedModel.ExpectedConstructors;
            }
        }
      
        public string ExpectedConstructorsString
        {
            get
            {
                return this.typedModel.ExpectedConstructors.ToNewLineSeparatedString();
            }
            set
            {
                this.typedModel.ExpectedConstructors.Clear();

                string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                this.typedModel.ExpectedConstructors.AddRange(lines);
            }
        }

        private void Delete()
        {
            if (this.ParentCollection != null)
            {
                this.ParentCollection.Remove(this.Model);
            }
        }
    }
}
