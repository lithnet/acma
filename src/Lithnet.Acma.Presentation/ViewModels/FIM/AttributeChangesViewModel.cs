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
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.Acma.TestEngine;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.Presentation
{
    public class AttributeChangesViewModel : ListViewModel<AttributeChangeViewModel, AttributeChange>
    {
        private ObjectModificationType objectModificationType;

        private AcmaSchemaObjectClass objectClass;

        private Func<IEnumerable<UnitTestStepObjectCreation>> allowedReferenceObjects;

        public AttributeChangesViewModel(IList<AttributeChange> model, ObjectModificationType objectModificationType, AcmaSchemaObjectClass objectClass, Func<IEnumerable<UnitTestStepObjectCreation>> allowedReferenceObjects)
            : base()
        {
            this.objectClass = objectClass;
            this.allowedReferenceObjects = allowedReferenceObjects;
            this.objectModificationType = objectModificationType;
            this.Commands.AddItem("AddAttributeChange", t => this.AddAttributeChange(), t => this.CanAddAttributeChange());
            ActiveConfig.DB.OnAttributeRenamed += DB_OnAttributeRename;
            ActiveConfig.DB.OnAttributeDeleted += DB_OnAttributeDeleted;
            ActiveConfig.DB.OnBindingDeleted += DB_OnBindingDeleted;

            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));

            if (this.Count > 0)
            {
                this.FirstOrDefault().IsSelected = true;
            }
        }

        private void DB_OnBindingDeleted(string objectClassName, string attributeName)
        {
            if (objectClassName != this.objectClass.Name)
            {
                return;
            }

            foreach (var item in this.ToList())
            {
                if (item.Model.Name == attributeName)
                {
                    this.Remove(item);
                }
            }
        }

        private void DB_OnAttributeDeleted(string name)
        {
            foreach (var item in this.ToList())
            {
                if (item.Model.Name == name)
                {
                    this.Remove(item);
                }
            }
        }

        private void DB_OnAttributeRename(AcmaSchemaAttribute attribute, string oldName)
        {
            foreach (var item in this.ToList())
            {
                if (item.Model.Name == oldName)
                {
                    AttributeChange change = new AttributeChangeDetached(attribute.Name, item.Model.ModificationType, item.Model.ValueChanges);
                    this.Remove(item);
                    this.Add(change);
                }
            }
        }

        private AttributeChangeViewModel ViewModelResolver(AttributeChange model)
        {
            return new AttributeChangeViewModel(model);
        }

        public IEnumerable<UnitTestStepObjectCreation> AllowedReferenceObjects
        {
            get
            {
                if (this.allowedReferenceObjects == null)
                {
                    return null;
                }
                else
                {
                    return this.allowedReferenceObjects.Invoke();
                }
            }
        }

        private IEnumerable<AcmaSchemaAttribute> GetAllowedAttributes()
        {
            IEnumerable<AcmaSchemaAttribute> attributesAllowed;
            IEnumerable<AcmaSchemaAttribute> attributesInUse = this.Where(t => ActiveConfig.DB.HasAttribute(t.Model.Name)).Select(t => ActiveConfig.DB.GetAttribute(t.Model.Name));
            attributesAllowed = this.objectClass.Attributes.Except(attributesInUse).OrderBy(t => t.Name);

            if (this.AllowedReferenceObjects == null || this.AllowedReferenceObjects.Count() == 0)
            {
                attributesAllowed = attributesAllowed.Where(t => t.Type != ExtendedAttributeType.Reference);
            }

            return attributesAllowed;
        }

        private IEnumerable<AttributeModificationType> GetAllowedModificationTypes()
        {
            switch (this.objectModificationType)
            {
                case ObjectModificationType.Add:
                    yield return AttributeModificationType.Add;
                    break;

                case ObjectModificationType.Delete:
                    yield break;

                case ObjectModificationType.None:
                    yield break;

                case ObjectModificationType.Replace:
                    yield return AttributeModificationType.Add;
                    break;

                case ObjectModificationType.Unconfigured:
                    yield break;

                case ObjectModificationType.Update:
                    yield return AttributeModificationType.Add;
                    yield return AttributeModificationType.Update;
                    yield return AttributeModificationType.Delete;
                    yield return AttributeModificationType.Replace;
                    break;

                default:
                    yield break;
            }
        }

        public void AddAttributeChange()
        {
            NewAttributeChangeWindow window = new NewAttributeChangeWindow();
            NewAttributeChangeViewModel vm = new NewAttributeChangeViewModel(window, this.GetAllowedAttributes(), this.GetAllowedModificationTypes());
            window.DataContext = vm;

            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                    AttributeChange newItem = Utils.CreateAttributeChange(vm.Attribute.Name, vm.ModificationType);
                    this.Add(newItem, true);

                    AttributeChangeViewModel newItemVM = this.ViewModelResolver(newItem);

                    if (newItemVM.ModificationType != AttributeModificationType.Delete)
                    {
                        //newItemVM.AddValueChange();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not create attribute change - " + ex.Message);
                }
            }
        }
       
        public bool CanAddAttributeChange()
        {
            if (this.objectModificationType == ObjectModificationType.Delete)
            {
                return false;
            }

            return this.GetAllowedAttributes().Any();
        }
    }
}
