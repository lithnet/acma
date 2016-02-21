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
    public class AttributeChangeViewModel : ViewModelBase<AttributeChange>
    {
        private AcmaSchemaAttribute attribute;

        private ValueChangesViewModel valueChanges;

        public IEnumerable<UnitTestStepObjectCreation> AllowedReferenceObjects
        {
            get
            {
                AttributeChangesViewModel vm = this.Parent as AttributeChangesViewModel;
                if (vm == null)
                {
                    return null;
                }
                else
                {
                    return vm.AllowedReferenceObjects;
                }
            }
        }

        public AttributeChangeViewModel(AttributeChange model)
            : base(model)
        {
            this.Commands.AddItem("AddAttributeChange", t => ((AttributeChangesViewModel)this.ParentCollection).AddAttributeChange(), t =>
            {
                if (this.ParentCollection == null)
                {
                    return false;
                }
                else
                {
                    return ((AttributeChangesViewModel)this.ParentCollection).CanAddAttributeChange();
                }
            });
            
            this.Commands.AddItem("DeleteAttributeChange", t => this.Delete());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.attribute = ActiveConfig.DB.GetAttribute(this.Name);
            this.ValueChanges = new ValueChangesViewModel(this.Model.ValueChanges, this);
            this.ValueChanges.CollectionChanged += ValueChanges_CollectionChanged;
        }

        public void AddValueChange()
        {
            this.ValueChanges.AddValueChangeAdd();
            this.RaisePropertyChanged("ValueChanges");
        }

        private void ValueChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged("ValueChangesString");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{{{0}}} {1}", this.Name, this.ModificationType.GetEnumDescription());
            }
        }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public bool IsMultivalued
        {
            get
            {
                return this.attribute.IsMultivalued;
            }
        }

        public ExtendedAttributeType Type
        {
            get
            {
                return this.attribute.Type;
            }
        }

        public AttributeModificationType ModificationType
        {
            get
            {
                return this.Model.ModificationType;
            }
        }

        public ValueChangesViewModel ValueChanges
        {
            get
            {
                return this.valueChanges;
            }
            set
            {
                if (this.valueChanges != null)
                {
                    this.UnregisterChildViewModel(this.valueChanges);
                }

                this.valueChanges = value;

                this.RegisterChildViewModel(this.valueChanges);
            }
        }

        public string ValueChangesString
        {
            get
            {
                return this.ValueChanges.ValueChangesString;
            }
        }

        private void Delete()
        {
            this.ParentCollection.Remove(this.Model);
        }

        private void AddValueChange(ValueModificationType type, object value)
        {
            switch (type)
            {
                case ValueModificationType.Add:
                    this.ValueChanges.Add(ValueChange.CreateValueAdd(value));
                    break;

                case ValueModificationType.Delete:
                    this.ValidateValueChangeType(type);
                    this.ValueChanges.Add(ValueChange.CreateValueDelete(value));

                    break;
                case ValueModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(type);
            }
        }

        private void ValidateValueChangeType(ValueModificationType type)
        {
            switch (this.ModificationType)
            {
                case AttributeModificationType.Add:
                    if (type != ValueModificationType.Add)
                    {
                        throw new UnknownOrUnsupportedModificationTypeException(type);
                    }

                    break;

                case AttributeModificationType.Delete:
                    throw new UnknownOrUnsupportedModificationTypeException(type);

                case AttributeModificationType.Replace:
                    if (type != ValueModificationType.Add)
                    {
                        throw new UnknownOrUnsupportedModificationTypeException(type);
                    }

                    break;

                case AttributeModificationType.Update:
                    break;

                default:
                case AttributeModificationType.Unconfigured:
                    throw new UnknownOrUnsupportedModificationTypeException(type);
            }
        }
    }
}
