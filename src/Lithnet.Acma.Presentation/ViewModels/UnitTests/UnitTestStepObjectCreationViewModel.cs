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
    public class UnitTestStepObjectCreationViewModel : UnitTestStepViewModel
    {
        private UnitTestStepObjectCreation typedModel;

        private AttributeChangesViewModel attributeChanges;

        public UnitTestStepObjectCreationViewModel(UnitTestStepObjectCreation model)
            : base(model)
        {
            this.typedModel = model;
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(model.ObjectClassName);
            this.AttributeChanges = new AttributeChangesViewModel(model.CSEntryChange.AttributeChanges, model.CSEntryChange.ObjectModificationType, objectClass, () => this.GetAllowedReferenceObjects());
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
                return string.Format("Create {0}", this.Name);
            }
        }

        public IEnumerable<UnitTestStepObjectCreation> GetAllowedReferenceObjects()
        {
            if (this.typedModel.ParentTest != null)
            {
                return this.typedModel.ParentTest.GetObjectCreationStepsBeforeItem(this.typedModel);
            }
            else
            {
                return null;
            }
        }

        public ObjectModificationType ModificationType
        {
            get
            {
                return this.typedModel.ModificationType;
            }
        }

        public string ObjectClassName
        {
            get
            {
                return this.typedModel.ObjectClassName;
            }
            set
            {
                this.typedModel.ObjectClassName = value;
            }
        }

        public Guid ObjectId
        {
            get
            {
                return this.typedModel.ObjectId;
            }
            set
            {
                this.typedModel.ObjectId = value;
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
