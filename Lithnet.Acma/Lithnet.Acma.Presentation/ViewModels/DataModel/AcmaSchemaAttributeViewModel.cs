using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;
using System.Collections;
using System.ComponentModel;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaAttributeViewModel : ViewModelBase<AcmaSchemaAttribute>
    {
        private AcmaSchemaAttribute model;

        public AcmaSchemaAttributeViewModel(AcmaSchemaAttribute model)
            : base(model)
        {
            this.Commands.AddItem("DeleteAttribute", t => this.DeleteAttribute(), t => this.CanDelete());
            this.Commands.AddItem("AddAttribute", t => ((AcmaSchemaAttributesViewModel)this.ParentCollection).AddAttribute());
            this.model = model;
            //this.AddDependentPropertyNotification("HasAnyChanges", "DisplayName");
            this.IgnorePropertyHasChanged.Add("DisplayName");

        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", this.Name, this.Type.ToString());
            }
        }

        public string Name
        {
            get
            {
                return this.model.Name;
            }
            set
            {
                if (value != this.model.Name)
                {
                    ActiveConfig.DB.RenameAttribute(this.model.Name, value);
                    ActiveConfig.DB.RefreshEntity(this.model);
                }
            }
        }

        public ExtendedAttributeType Type
        {
            get
            {
                return this.model.Type;
            }
        }

        public string TypeName
        {
            get
            {
                return this.model.Type.ToString() + ((this.Type == ExtendedAttributeType.String || this.Type == ExtendedAttributeType.Binary) && this.IsIndexable ? " (indexable)" : string.Empty);
            }
        }

        public AcmaAttributeOperation Operation
        {
            get
            {
                return this.model.Operation;
            }
            set
            {
                if (this.model.Operation != value)
                {
                    this.model.Operation = value;
                    ActiveConfig.DB.Commit();
                }
            }
        }

        public bool IsMultivalued
        {
            get
            {
                return this.model.IsMultivalued;
            }
        }

        public bool IsIndexed
        {
            get
            {
                if (this.model.IsInAVPTable && this.model.IsIndexable)
                {
                    return true;
                }
                else
                {
                    return this.model.IsIndexed;
                }
            }
            set
            {
                if (this.model.IsIndexed != value)
                {
                    if (value)
                    {
                        ActiveConfig.DB.CreateIndex(this.model);
                    }
                    else
                    {
                        ActiveConfig.DB.DeleteIndex(this.model);
                    }

                    this.model.IsIndexed = value;
                }
            }
        }

        public bool IsIndexable
        {
            get
            {
                if (this.IsBuiltIn || this.Type == ExtendedAttributeType.Reference)
                {
                    return false;
                }
                else
                {
                    return this.model.IsIndexable;
                }
            }
        }

        public bool CanChangeIndex
        {
            get
            {
                if (this.IsBuiltIn || this.Type == ExtendedAttributeType.Reference || !this.model.IsIndexable || (this.model.IsInAVPTable && this.model.IsIndexable))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsBuiltIn
        {
            get
            {
                return this.model.IsBuiltIn;
            }
        }

        public bool CanModify
        {
            get
            {
                return !this.model.IsBuiltIn;
            }
        }

        public int Bindings
        {
            get
            {
                return this.model.Mappings.Count;
            }
        }

        public int BackLinks
        {
            get
            {
                return this.model.BackLinks.Count;
            }
        }

        public int ForwardLinks
        {
            get
            {
                return this.model.ForwardLinks.Count;
            }
        }


        private void DeleteAttribute()
        {
            try
            {
                if (MessageBox.Show("Are you are you want to delete this attribute?\n\nAll data stored in this attribute will be deleted. This operation cannot be undone", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    ActiveConfig.DB.DeleteAttribute(this.model);
                    this.ParentCollection.Remove(this.model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the attribute\n\n" + ex.Message);
            }
        }

        private bool CanDelete()
        {
            return ActiveConfig.DB.CanDeleteAttribute(this.model);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is AcmaSchemaAttributeViewModel)
            {
                AcmaSchemaAttributeViewModel otherAttribute = (AcmaSchemaAttributeViewModel)obj;

                if (this.Model == otherAttribute.Model)
                {
                    return true;
                }
            }

            return base.Equals(obj);
        }

        public static bool operator ==(AcmaSchemaAttributeViewModel a, AcmaSchemaAttributeViewModel b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Model == b.Model;
        }

        public static bool operator !=(AcmaSchemaAttributeViewModel a, AcmaSchemaAttributeViewModel b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Model.GetHashCode();
        }

    }
}
