using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.TestEngine;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestStepsViewModel : ListViewModel<UnitTestStepViewModel, UnitTestStep>
    {
        private UnitTest parentUnitTest;

        public UnitTestStepsViewModel(IList<UnitTestStep> model, UnitTest parentUnitTest)
            : base()
        {
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.parentUnitTest = parentUnitTest;
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddUnitTestObjectEvaluation", t => this.AddUnitTestObjectEvaluation(), t => this.CanAddUnitTestObjectEvaluation());
            this.Commands.AddItem("AddUnitTestObjectModification", t => this.AddUnitTestObjectModification(), t => this.CanAddUnitTestObjectModification());
            this.Commands.AddItem("AddUnitTestObjectCreation", t => this.AddUnitTestObjectCreation());
            this.Commands.AddItem("Paste", t => this.Paste(), t => this.CanPaste());
            ActiveConfig.DB.OnObjectClassDeleted += DB_OnObjectClassDeleted;
            this.OnModelRemoved += UnitTestStepsViewModel_OnModelRemoved;
            this.PasteableTypes.Add(typeof(UnitTestStepObjectCreation));
            this.PasteableTypes.Add(typeof(UnitTestStepObjectEvaluation));
            this.PasteableTypes.Add(typeof(UnitTestStepObjectModification));
        }

        private void UnitTestStepsViewModel_OnModelRemoved(ListViewModelChangedEventArgs args)
        {
            if (args.Model is UnitTestStepObjectCreation)
            {
                UnitTestStepObjectCreation step = (UnitTestStepObjectCreation)args.Model;

                foreach (UnitTestStepObjectModificationViewModel modStep in this.OfType<UnitTestStepObjectModificationViewModel>().ToList<UnitTestStepObjectModificationViewModel>())
                {
                    if (modStep.ObjectCreationStep == step)
                    {
                        this.Remove(modStep);
                    }
                    else
                    {
                        foreach (AttributeChangeViewModel change in modStep.AttributeChanges.Where(t => t.Model.DataType == AttributeType.Reference).ToList())
                        {
                            foreach (ValueChangeViewModel value in change.ValueChanges.ToList())
                            {
                                if (value.Value == step.ObjectId.ToString())
                                {
                                    change.ValueChanges.Remove(value);
                                }

                                if (change.ValueChanges.Count == 0)
                                {
                                    modStep.AttributeChanges.Remove(change);
                                }
                            }
                        }
                    }
                }

                foreach (UnitTestStepObjectEvaluationViewModel evalStep in this.OfType<UnitTestStepObjectEvaluationViewModel>().ToList<UnitTestStepObjectEvaluationViewModel>())
                {
                    if (evalStep.ObjectCreationStep == step)
                    {
                        this.Remove(evalStep);
                    }
                }

                foreach (UnitTestStepObjectCreationViewModel createStep in this.OfType<UnitTestStepObjectCreationViewModel>().ToList<UnitTestStepObjectCreationViewModel>())
                {
                    foreach (AttributeChangeViewModel change in createStep.AttributeChanges.Where(t => t.Model.DataType == AttributeType.Reference).ToList())
                    {
                        foreach (ValueChangeViewModel value in change.ValueChanges.ToList())
                        {
                            if (value.Value == step.ObjectId.ToString())
                            {
                                change.ValueChanges.Remove(value);
                            }

                            if (change.ValueChanges.Count == 0)
                            {
                                createStep.AttributeChanges.Remove(change);
                            }
                        }
                    }
                }

            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Steps");
            }
        }

        private void DB_OnObjectClassDeleted(string objectClassName)
        {
            foreach (var item in this.Models.OfType<UnitTestStepObjectModification>().ToList())
            {
                if (item.ObjectClassName == objectClassName)
                {
                    this.Remove(item);
                }
            }

            foreach (var item in this.Models.OfType<UnitTestStepObjectCreation>().ToList())
            {
                if (item.ObjectClassName == objectClassName)
                {
                    this.Remove(item);
                }
            }
        }

        private UnitTestStepViewModel ViewModelResolver(UnitTestStep model)
        {
            if (model.GetType() == typeof(UnitTestStepObjectEvaluation))
            {
                return new UnitTestStepObjectEvaluationViewModel(model as UnitTestStepObjectEvaluation, this.parentUnitTest);
            }
            else if (model.GetType() == typeof(UnitTestStepObjectModification))
            {
                return new UnitTestStepObjectModificationViewModel(model as UnitTestStepObjectModification);
            }
            else if (model.GetType() == typeof(UnitTestStepObjectCreation))
            {
                return new UnitTestStepObjectCreationViewModel(model as UnitTestStepObjectCreation);
            }
            else
            {
                throw new InvalidOperationException("The unit test step object type was unknown");
            }
        }

        public void AddUnitTestObjectEvaluation()
        {
            UnitTestStepObjectEvaluation step = new UnitTestStepObjectEvaluation();
            step.ParentTest = this.parentUnitTest;
            step.ObjectCreationStep = this.Models.OfType<UnitTestStepObjectCreation>().FirstOrDefault();
            step.Name = "Evaluate";
            this.Add(step, true);
        }

        public bool CanAddUnitTestObjectEvaluation()
        {
            return this.Any(t => t is UnitTestStepObjectCreationViewModel);
        }

        public void AddUnitTestObjectCreation()
        {
            NewObjectCreationWindow window = new NewObjectCreationWindow();
            NewObjectCreationViewModel vm = new NewObjectCreationViewModel(window);
            window.DataContext = vm;
            vm.StepName = vm.ObjectClass.Name;

            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                UnitTestStepObjectCreation step = new UnitTestStepObjectCreation();
                step.ParentTest = this.parentUnitTest;
                step.ObjectId = Guid.NewGuid();
                step.Name = vm.StepName;
                step.ObjectClassName = vm.ObjectClass.Name;
                step.ModificationType = ObjectModificationType.Add;
                this.Add(step, true);
            }
        }

        public void AddUnitTestObjectModification()
        {

            NewObjectModificationWindow window = new NewObjectModificationWindow();
            NewObjectModificationViewModel vm = new NewObjectModificationViewModel(window, this.parentUnitTest);
            window.DataContext = vm;
            vm.StepName = "Modify";
            vm.ModificationType = ObjectModificationType.Update;

            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                UnitTestStepObjectModification step = new UnitTestStepObjectModification();
                step.ParentTest = this.parentUnitTest;
                step.ObjectCreationStep = vm.CreationObject;
                step.Name = vm.StepName;
                step.ModificationType = vm.ModificationType;
                this.Add(step, true);
            }
        }
        
        public bool CanAddUnitTestObjectModification()
        {
            return this.Any(t => t is UnitTestStepObjectCreationViewModel);
        }
    }
}
