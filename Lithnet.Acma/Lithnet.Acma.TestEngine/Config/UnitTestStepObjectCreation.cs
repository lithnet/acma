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

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "object-creation", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class UnitTestStepObjectCreation : UnitTestStep
    {
        private bool deserializing;

        [DataMember(Name = "cs-entry")]
        public AcmaCSEntryChange CSEntryChange { get; set; }

        [DataMember(Name = "test-name")]
        public override string Name { get; set; }

        public Guid ObjectId
        {
            get
            {
                if (this.CSEntryChange != null)
                {
                    Guid result;

                    if (!Guid.TryParse(this.CSEntryChange.DN, out result))
                    {
                        return Guid.Empty;
                    }

                    return result;
                }
                else
                {
                    return Guid.Empty;
                }
            }
            set
            {
                if (this.CSEntryChange != null)
                {
                    this.CSEntryChange.DN = value.ToString();
                }
                else
                {
                    throw new InvalidOperationException("There is no CSEntryChange present");
                }
            }
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                if (this.ObjectClassName == null)
                {
                    return null;
                }
                else
                {
                    return ActiveConfig.DB.GetObjectClass(this.ObjectClassName);
                }
            }
            set
            {
                if (value == null)
                {
                    this.CSEntryChange.ObjectType = null;
                }
                else
                {
                    this.CSEntryChange.ObjectType = value.Name;
                }
            }
        }

        public string ObjectClassName
        {
            get
            {
                return this.CSEntryChange.ObjectType;
            }
            set
            {
                this.CSEntryChange.ObjectType = value;
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

        public UnitTestStepObjectCreation()
        {
            this.Initialize();
        }

        public override void Execute(MADataContext dc)
        {
            if (this.CSEntryChange.ObjectModificationType != ObjectModificationType.Add)
            {
                if (this.CSEntryChange.ObjectModificationType == ObjectModificationType.Unconfigured)
                {
                    this.CSEntryChange.ObjectModificationType = ObjectModificationType.Add;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("The CSEntryChange in unit test step {0} contained an invalid modification type", this.Name));
                }
            }

            bool refRetryRequired = false;
            
            CSEntryExport.PutExportEntry(this.CSEntryChange, dc, out refRetryRequired);
            
            if (refRetryRequired)
            {
                throw new ReferencedObjectNotPresentException();
            }
        }

        public override void Cleanup(MADataContext dc)
        {
            if (this.CSEntryChange != null)
            {
                Guid dn = new Guid(this.CSEntryChange.DN);
                dc.DeleteMAObjectPermanent(dn);
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            if (this.deserializing)
            {
                return;
            }

            base.ValidatePropertyChange(propertyName);

            if (propertyName == "ModificationType")
            {
                if (this.ModificationType != ObjectModificationType.Add &&
                    this.ModificationType != ObjectModificationType.Unconfigured)
                {
                    this.AddError("ModificationType", "The modification type is unsupported");
                }
                else
                {
                    this.RemoveError("ModificationType");
                }
            }
            else if (propertyName == "ObjectClass")
            {
                if (this.ObjectClass == null)
                {
                    this.AddError("ObjectClass", "An object class must be specified");
                }
                else
                {
                    this.RemoveError("ObjectClass");
                }
            }
        }

        private void Initialize()
        {
            this.CSEntryChange = new AcmaCSEntryChange();
            this.CSEntryChange.ObjectModificationType = ObjectModificationType.Add;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;
            this.ValidatePropertyChange("ObjectClass");
            this.ValidatePropertyChange("ModificationType");
        }
    }
}
