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
using Lithnet.Common.ObjectModel;
using System.Collections.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class ValueDeclarationsViewModel : ListViewModel<ValueDeclarationViewModel, ValueDeclaration>, IEditableCollectionView
    {
        private IList<ValueDeclaration> typedModel;

        private AcmaSchemaObjectClass objectClass;

        public ValueDeclarationsViewModel(IList<ValueDeclaration> model, AcmaSchemaObjectClass objectClass)
            : base()
        {
            this.typedModel = model;
            this.objectClass = objectClass;
            this.SetCollectionViewModel((IList)model, t => this.ViewModelResolver(t));
            this.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtEnd;
        }

        private ValueDeclarationViewModel ViewModelResolver(ValueDeclaration model)
        {
            return new ValueDeclarationViewModel(model, this.objectClass);
        }

        
        protected override void ViewModelAdded(ValueDeclarationViewModel viewModel)
        {
            ((ValueDeclarationViewModel)viewModel).ObjectClass = this.objectClass;
            this.SubscribeToErrors(viewModel);
        }

        private string bindingTransactionDeclarationText;

        private string bindingTransactionTransformText;

        public object AddNewItem(object newItem)
        {
            if (!(newItem is ValueDeclarationViewModel))
            {
                throw new InvalidOperationException("Unknown object type");
            }

            this.Add((ValueDeclarationViewModel)newItem);

            return newItem;
        }

        public bool CanAddNewItem
        {
            get
            {
                return true;
            }
        }

        public object AddNew()
        {
            ValueDeclarationViewModel newItem = new ValueDeclarationViewModel();
            newItem.ObjectClass = this.objectClass;
            this.CurrentAddItem = newItem;
            return newItem;
        }

        public bool CanAddNew
        {
            get
            {
                return true;
            }
        }

        public bool CanCancelEdit
        {
            get
            {
                return this.CurrentEditItem != null;
            }
        }

        public bool CanRemove
        {
            get
            {
                return this.ViewModels.Count > 1;
            }
        }

        public void CancelEdit()
        {
            if (this.CurrentEditItem != null)
            {
                ((ValueDeclarationViewModel)this.CurrentEditItem).Declaration = this.bindingTransactionDeclarationText;
                ((ValueDeclarationViewModel)this.CurrentEditItem).TransformsString = this.bindingTransactionTransformText;
            }

            this.CurrentEditItem = null;
        }

        public void CancelNew()
        {
            this.CurrentAddItem = null;
        }

        public void CommitEdit()
        {
            this.CurrentEditItem = null;
        }

        public void CommitNew()
        {
            if (this.CurrentAddItem != null)
            {
                this.Add((ValueDeclarationViewModel)this.CurrentAddItem);
                this.CurrentAddItem = null;
            }
            else
            {
                throw new InvalidOperationException("There is no new object to commit");
            }
        }

        public object CurrentAddItem { get; private set; }

        public object CurrentEditItem { get; private set; }

        public void EditItem(object item)
        {
            if (this.CurrentEditItem != null)
            {
                throw new InvalidOperationException("There is another transaction already in progress");
            }

            this.bindingTransactionDeclarationText = ((ValueDeclarationViewModel)this.CurrentEditItem).Declaration;
            this.bindingTransactionTransformText = ((ValueDeclarationViewModel)this.CurrentEditItem).TransformsString;

            this.CurrentEditItem = item;
        }

        public bool IsAddingNew { get; private set; }

        public bool IsEditingItem { get; private set; }

        public NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }

        public void Remove(object item)
        {
            if (!(item is ValueDeclarationViewModel))
            {
                throw new InvalidOperationException("Unknown object type");
            }

            this.Remove((ValueDeclarationViewModel)item);

        }

    }
}
