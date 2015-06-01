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
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.Acma.TestEngine;

namespace Lithnet.Acma.Presentation
{
    public class ValueChangeViewModel : ViewModelBase<ValueChange>
    {
        public ValueChangeViewModel(ValueChange model)
            : base(model)
        {
            this.Commands.AddItem("AddValueChangeDelete", t => ((ValueChangesViewModel)this.ParentCollection).AddValueChangeDelete(), t =>
            {
                if (this.ParentCollection == null)
                {
                    return false;
                }
                else
                {
                    return ((ValueChangesViewModel)this.ParentCollection).CanAddValueChangeDelete();
                }
            });

            this.Commands.AddItem("AddValueChangeAdd", t => ((ValueChangesViewModel)this.ParentCollection).AddValueChangeAdd(), t => {
                if (this.ParentCollection == null)
                {
                    return false;
                }
                else
                {
                    return ((ValueChangesViewModel)this.ParentCollection).CanAddValueChangeAdd();
                }
            });

            this.Commands.AddItem("DeleteValueChange", t => this.Delete());
            this.Commands.AddItem("EditValueChange", t => this.EditValueChange());

            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0}:{1}", this.ModificationType.GetEnumDescription().ToLower(), this.Value.ToSmartStringOrEmptyString());
            }
        }

        public string Value
        {
            get
            {
                ValueChangesViewModel parent = this.Parent as ValueChangesViewModel;
                if (parent != null)
                {
                    if (parent.ParentAttributeChange != null)
                    {
                        if (parent.ParentAttributeChange.Type == ExtendedAttributeType.Reference)
                        {
                            foreach (var item in parent.ParentAttributeChange.AllowedReferenceObjects.OfType<UnitTestStepObjectCreation>())
                            {
                                if (item.ObjectId.ToString() == this.Model.Value.ToSmartStringOrNull())
                                {
                                    return item.Name;
                                }
                            }
                        }
                    }
                }

                return this.Model.Value.ToSmartStringOrNull();
            }
        }

        internal object ModelValue
        {
            get
            {
                return this.Model.Value;
            }
        }

        public ValueModificationType ModificationType
        {
            get
            {
                return this.Model.ModificationType;
            }
        }

        private void Delete()
        {
            this.ParentCollection.Remove(this.Model);
        }

        internal void EditValueChange()
        {
            ((ValueChangesViewModel)this.ParentCollection).EditValueChange();
        }
    }
}
