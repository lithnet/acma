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
using System.Collections;

namespace Lithnet.Acma.Presentation.ViewModels.FIM
{
    public class CSEntryChangeViewModel : ViewModelBase<CSEntryChange>
    {
        AcmaSchemaObjectClass objectClass;

        AttributeChangesViewModel attributeChanges;

        public CSEntryChangeViewModel(CSEntryChange model)
            : base(model)
        {
            this.Commands.AddItem("Delete", t => this.Delete());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.objectClass = ActiveConfig.DB.GetObjectClass(model.ObjectType);
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} {1}", this.DN, this.ModificationType.GetEnumDescription());
            }
        }

        public string DN
        {
            get
            {
                return this.Model.DN;
            }
            set
            {
                this.Model.DN = value;
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
                this.objectClass = value;
                this.Model.ObjectType = value.Name;
            }
        }

        public ObjectModificationType ModificationType
        {
            get
            {
                return this.Model.ObjectModificationType;
            }
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.AttributeChanges;
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


        private void Delete()
        {

        }

        private void AddAttributeChange(AttributeModificationType type, AcmaSchemaAttribute attribute)
        {
            this.ValidateAttributeChangeType(type);
            
            switch (type)
            {
                case AttributeModificationType.Replace:
                case AttributeModificationType.Update:
                case AttributeModificationType.Delete:
                case AttributeModificationType.Add:
                    this.AttributeChanges.Add(Utils.CreateAttributeChange(attribute.Name, type));
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(type);
            }
        }

        private void ValidateAttributeChangeType(AttributeModificationType type)
        {
            DetachedUtils.ValidateAttributeModificationType(this.ModificationType, type);
        }
    }
}
