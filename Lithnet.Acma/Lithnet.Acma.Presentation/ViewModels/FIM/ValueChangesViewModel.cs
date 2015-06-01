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
using Lithnet.Common.ObjectModel;
using Lithnet.Fim.Core;

namespace Lithnet.Acma.Presentation
{
    public class ValueChangesViewModel : ListViewModel<ValueChangeViewModel, ValueChange>
    {
        public AttributeChangeViewModel ParentAttributeChange { get; private set; }

        public ValueChangesViewModel(IList<ValueChange> model, AttributeChangeViewModel parent)
            : base()
        {
            this.ParentAttributeChange = parent;
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));

            this.Commands.AddItem("AddValueChangeAdd", t => this.AddValueChangeAdd(), t => this.CanAddValueChangeAdd());
            this.Commands.AddItem("AddValueChangeDelete", t => this.AddValueChangeDelete(), t => this.CanAddValueChangeDelete());
            this.Commands.AddItem("EditValueChange", t => this.EditValueChange(), t => this.CanEditValueChange());

            if (this.Count > 0)
            {
                this.FirstOrDefault().IsSelected = true;
            }
        }

        private ValueChangeViewModel ViewModelResolver(ValueChange model)
        {
            return new ValueChangeViewModel(model);
        }

        public string ValueChangesString
        {
            get
            {
                List<string> changes = new List<string>();
                foreach (ValueChangeViewModel change in this)
                {
                    if (this.ParentAttributeChange.ModificationType == AttributeModificationType.Update)
                    {
                        changes.Add(change.DisplayName);
                    }
                    else
                    {
                        changes.Add(change.Value.ToSmartString());
                    }
                }

                return changes.ToCommaSeparatedString();
            }
        }

        public void AddValueChangeAdd()
        {
            NewValueChangeWindow window = new NewValueChangeWindow();
            NewValueChangeViewModel vm = new NewValueChangeViewModel(
                window,
                this.ParentAttributeChange.AllowedReferenceObjects,
                 this.ParentAttributeChange.Type,
                 ValueModificationType.Add
                );

            window.DataContext = vm;

            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                    this.Add(ValueChange.CreateValueAdd(Lithnet.Fim.Core.TypeConverter.ConvertData(vm.Value, this.ParentAttributeChange.Type)), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not create value change - " + ex.Message);
                }
            }
        }

        public void EditValueChange()
        {
            ValueChangeViewModel selected = this.ViewModels.FirstOrDefault(t => t.IsSelected);

            if (selected == null)
            {
                return;
            }

            NewValueChangeWindow window = new NewValueChangeWindow();
            NewValueChangeViewModel vm = new NewValueChangeViewModel(window,
                this.ParentAttributeChange.AllowedReferenceObjects,
                this.ParentAttributeChange.Type,
                selected.ModificationType,
                selected);

            window.DataContext = vm;

            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                    this.Add(ValueChange.CreateValueAdd(Lithnet.Fim.Core.TypeConverter.ConvertData(vm.Value, this.ParentAttributeChange.Type)), true);
                    this.Remove(selected);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not create value change - " + ex.Message);
                }
            }

        }

        public bool CanEditValueChange()
        {
            return this.ViewModels.Any(t => t.IsSelected);
        }

        public bool CanAddValueChangeAdd()
        {
            if (this.ParentAttributeChange.ModificationType == AttributeModificationType.Delete)
            {
                return false;
            }

            if (!this.ParentAttributeChange.IsMultivalued)
            {
                if (this.Any(t => t.Model.ModificationType == ValueModificationType.Add))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        public void AddValueChangeDelete()
        {
            NewValueChangeWindow window = new NewValueChangeWindow();
            NewValueChangeViewModel vm = new NewValueChangeViewModel(
                window,
                this.ParentAttributeChange.AllowedReferenceObjects,
                this.ParentAttributeChange.Type,
                ValueModificationType.Delete);

            window.DataContext = vm;

            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                    this.Add(ValueChange.CreateValueDelete(Lithnet.Fim.Core.TypeConverter.ConvertData(vm.Value, this.ParentAttributeChange.Type)), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not create value change - " + ex.Message);
                }
            }
        }

        public bool CanAddValueChangeDelete()
        {
            if (this.ParentAttributeChange.ModificationType != AttributeModificationType.Update)
            {
                return false;
            }

            if (!this.ParentAttributeChange.IsMultivalued)
            {
                if (this.Any(t => t.Model.ModificationType == ValueModificationType.Delete))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    }
}
