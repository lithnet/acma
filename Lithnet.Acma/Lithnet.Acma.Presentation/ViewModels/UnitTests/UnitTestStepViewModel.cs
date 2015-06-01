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
using System.Windows.Media.Imaging;
using System.Windows;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestStepViewModel : ViewModelBase<UnitTestStep>
    {
        public UnitTestStepViewModel(UnitTestStep model)
            : base(model)
        {
            this.Commands.AddItem("DeleteStep", t => this.DeleteStep(), t => this.CanDeleteStep());
            this.EnableCutCopy();
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public int ParentIndex
        {
            get
            {
                return this.ParentCollection.IndexOf(this.Model);
            }
        }

        protected override bool CanMoveUp()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            
            return true;

        }


        protected override bool CanMoveDown()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }

            return true;

        }

        private void DeleteStep()
        {
            if (this is UnitTestStepObjectCreationViewModel)
            {
                if (MessageBox.Show("Deleting this step will delete any modification and evaluation steps for this object, as well as any attribute changes that reference this object in other steps. Are you sure you want to proceed?", "Delete object", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.No)
                {
                    return;
                }
            }

            this.ParentCollection.Remove(this.Model);
        }

        private bool CanDeleteStep()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }

            if (this.Model is UnitTestStepObjectCreation)
            {
                foreach (var item in this.ParentCollection.OfType<UnitTestStepViewModel>().Select(t => t.Model))
                {
                    if (item == this.Model)
                    {
                        continue;
                    }

                    if (item is UnitTestStepObjectModification)
                    {
                        if (((UnitTestStepObjectModification)item).ObjectCreationStep == this.Model)
                        {
                            return false;
                        }
                    }

                    if (item is UnitTestStepObjectEvaluation)
                    {
                        if (((UnitTestStepObjectEvaluation)item).ObjectCreationStep == this.Model)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override bool CanCut()
        {
            return this.CanDeleteStep();
        }
    }
}
