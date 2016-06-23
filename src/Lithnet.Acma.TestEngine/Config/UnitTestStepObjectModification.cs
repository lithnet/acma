using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MetadirectoryServices;
using System.Xml;
using Lithnet.Acma;
using System.Runtime.Serialization;
using System.ComponentModel;
using Lithnet.Acma.DataModel;
using Lithnet.Acma.ServiceModel;

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "object-modification", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class UnitTestStepObjectModification : UnitTestStep
    {
        [DataMember(Name = "cs-entry")]
        public AcmaCSEntryChange CSEntryChange { get; set; }

        private UnitTestStepObjectCreation objectCreationStep;

        private string objectCreationStepName;

        [DataMember(Name = "object-creation-step-name")]
        public string ObjectCreationStepName
        {
            get
            {
                if (this.objectCreationStep == null)
                {
                    return this.objectCreationStepName;
                }
                else
                {
                    return this.objectCreationStep.Name;
                }
            }
            private set
            {
                this.objectCreationStepName = value;
            }
        }

        public UnitTestStepObjectCreation ObjectCreationStep
        {
            get
            {
                if (this.objectCreationStep == null)
                {
                    if (this.ObjectCreationStepName != null)
                    {
                        this.objectCreationStep = this.ParentTest.GetObjectCreationStepsBeforeItem(this)
                            .FirstOrDefault(t => t.Name == this.ObjectCreationStepName);
                    }
                    else
                    {
                        return null;
                    }
                }

                return this.objectCreationStep;
            }
            set
            {
                this.objectCreationStep = value;
            }
        }

        [DataMember(Name = "test-name")]
        public override string Name { get; set; }

        public Guid ObjectId
        {
            get
            {
                return this.ObjectCreationStep.ObjectId;
            }
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.ObjectCreationStep.ObjectClass;
            }
        }

        public string ObjectClassName
        {
            get
            {
                return this.ObjectCreationStep.ObjectClassName;
            }
        }

        public ObjectModificationType ModificationType
        {
            get
            {
                if (this.CSEntryChange != null)
                {
                    return this.CSEntryChange.ObjectModificationType;
                }
                else
                {
                    return ObjectModificationType.Unconfigured;
                }
            }
            set
            {
                if (this.CSEntryChange != null)
                {
                    this.CSEntryChange.ObjectModificationType = value;
                }
                else
                {
                    throw new InvalidOperationException("There is no CSEntryChange present");
                }
            }
        }

        public UnitTestStepObjectModification()
        {
            this.CSEntryChange = new AcmaCSEntryChange();
        }

        public override void Execute()
        {
            bool refRetryRequired = false;
            this.PrepCSEntryChange();

            CSEntryExport.PutExportEntry(this.CSEntryChange, out refRetryRequired);

            if (refRetryRequired)
            {
                throw new ReferencedObjectNotPresentException();
            }
        }

        public override void Cleanup()
        {
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "ObjectCreationStep" || propertyName == "ParentTest")
            {
                if (this.ParentTest != null)
                {
                    if (this.ObjectCreationStep == null)
                    {
                        this.AddError("ObjectCreationStep", "An object creation step must be specified");
                    }
                    else
                    {
                        this.RemoveError("ObjectCreationStep");
                    }
                }
            }
            else if (propertyName == "ModificationType")
            {
                if (this.ModificationType == ObjectModificationType.None ||
                    this.ModificationType == ObjectModificationType.Add ||
                    this.ModificationType == ObjectModificationType.Unconfigured)
                {
                    this.AddError("ModificationType", "The modification type is unsupported");
                }
                else
                {
                    this.RemoveError("ModificationType");
                }
            }
        }

        private void PrepCSEntryChange()
        {
            this.CSEntryChange.DN = this.ObjectCreationStep.ObjectId.ToString();
            this.CSEntryChange.ObjectType = this.ObjectClassName;
        }

        private void Initialize()
        {
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (this.CSEntryChange == null)
            {
                this.CSEntryChange = new AcmaCSEntryChange();
            }
        }
    }
}
