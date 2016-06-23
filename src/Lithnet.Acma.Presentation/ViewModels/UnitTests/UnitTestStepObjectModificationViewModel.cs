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

namespace Lithnet.Acma.Presentation
{
    public class UnitTestStepObjectModificationViewModel : UnitTestStepViewModel
    {
        private UnitTestStepObjectModification typedModel;

        private AttributeChangesViewModel attributeChanges;

        public UnitTestStepObjectModificationViewModel(UnitTestStepObjectModification model)
            : base(model)
        {
            this.typedModel = model;
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(model.ObjectClassName);

            this.AttributeChanges = new AttributeChangesViewModel(model.CSEntryChange.AttributeChanges, model.CSEntryChange.ObjectModificationType, objectClass, () => this.GetAllowedReferenceObjects());
           
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
                return string.Format("{0} ({1})", this.Name, this.ObjectCreationStep.Name);
            }
        }

        public ObjectModificationType ModificationType
        {
            get
            {
                return this.typedModel.ModificationType;
            }
        }

        public string ObjectClass
        {
            get
            {
                return this.typedModel.ObjectClassName;
            }
        }

        public UnitTestStepObjectCreation ObjectCreationStep
        {
            get
            {
                return this.typedModel.ObjectCreationStep;
            }
        }

        public IEnumerable<UnitTestStepObjectCreation> GetAllowedReferenceObjects()
        {
            return this.typedModel.ParentTest.GetObjectCreationStepsBeforeItem(this.typedModel);
        }

        private void ObjectCreationStep_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }


        public Guid ObjectId
        {
            get
            {
                return this.typedModel.ObjectId;
            }
        }

        public IEnumerable<UnitTestStepObjectCreation> AllowedObjects
        {
            get
            {
                return this.Model.ParentTest.GetObjectCreationStepsBeforeItem(this.Model);
            }
        }

        public AttributeChangesViewModel AttributeChanges
        {
            get
            {
                return this.attributeChanges;
            }
            set
            {
                if (this.attributeChanges != null)
                {
                    this.UnregisterChildViewModel(this.attributeChanges);
                }

                this.attributeChanges = value;
                this.RegisterChildViewModel(attributeChanges);
            }
        }

        public bool IsAddingObject
        {
            get
            {
                return this.ModificationType == ObjectModificationType.Add;
            }
        }

        public bool IsChangingObject
        {
            get
            {
                return this.ModificationType == ObjectModificationType.Update ||
                    this.ModificationType == ObjectModificationType.Replace ||
                    this.ModificationType == ObjectModificationType.Delete;
            }
        }

        public bool IsEditingObject
        {
            get
            {
                return this.ModificationType == ObjectModificationType.Update ||
                        this.ModificationType == ObjectModificationType.Replace;
            }
        }

        public bool IsDeletingObject
        {
            get
            {
                return this.ModificationType == ObjectModificationType.Delete;
            }
        }
    }
}
