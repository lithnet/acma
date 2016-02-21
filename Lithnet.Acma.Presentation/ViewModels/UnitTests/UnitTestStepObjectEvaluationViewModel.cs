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
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.Acma.TestEngine;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestStepObjectEvaluationViewModel : UnitTestStepViewModel
    {
        private RuleGroupViewModel successCriteria;

        private UnitTestStepObjectEvaluation typedModel;

        private UnitTest parentModel;

        public UnitTestStepObjectEvaluationViewModel(UnitTestStepObjectEvaluation model, UnitTest parent)
            : base(model)
        {
            this.parentModel = parent;
            this.typedModel = model;
            this.SuccessCriteria = new RuleGroupViewModel(this.typedModel.SuccessCriteria, false);

            if (this.typedModel.ObjectCreationStep != null)
            {
                this.typedModel.ObjectCreationStep.PropertyChanged += ObjectCreationStep_PropertyChanged;
            }
        }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
            set
            {
                this.Model.Name = value;
            }
        }

        public string DisplayName
        {
            get
            {
                if (this.ObjectCreationStep == null)
                {
                    return string.Format("{0}", this.Name);
                }
                else
                {
                    return string.Format("{0} ({1})", this.Name, this.ObjectCreationStep.Name);
                }
            }
        }

        public IEnumerable<UnitTestStepObjectCreation> AllowedObjects
        {
            get
            {
                return this.Model.ParentTest.GetObjectCreationStepsBeforeItem(this.Model);
            }
        }

        public UnitTestStepObjectCreation ObjectCreationStep
        {
            get
            {
                return this.typedModel.ObjectCreationStep;
            }
            set
            {
                if (this.typedModel.ObjectCreationStep != null)
                {
                    this.typedModel.ObjectCreationStep.PropertyChanged -= ObjectCreationStep_PropertyChanged;
                }

                this.typedModel.ObjectCreationStep = value;

                if (this.typedModel.ObjectCreationStep != null)
                {
                    this.typedModel.ObjectCreationStep.PropertyChanged += ObjectCreationStep_PropertyChanged;
                }
            }
        }

        private void ObjectCreationStep_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }

        public RuleGroupViewModel SuccessCriteria
        {
            get
            {
                return this.successCriteria;
            }
            set
            {
                if (this.successCriteria != null)
                {
                    this.UnregisterChildViewModel(this.successCriteria);
                }

                this.successCriteria = value;

                this.RegisterChildViewModel(this.successCriteria);
            }
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.SuccessCriteria;
            }
        }
    }
}
