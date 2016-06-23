using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaReferenceLinkViewModel : ViewModelBase<AcmaSchemaReferenceLink>
    {
        private AcmaSchemaReferenceLinksViewModel parentVM;

        public AcmaSchemaReferenceLinkViewModel(AcmaSchemaReferenceLink model, AcmaSchemaReferenceLinksViewModel parentVM)
            : base(model)
        {
            this.parentVM = parentVM;

            this.Commands.AddItem("DeleteLink", t => this.DeleteLink(), t => this.CanDeleteLink());
            this.Commands.AddItem("AddLink", t => this.parentVM.CreateLink());
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format(
                    "{0}->{1}",
                    this.ForwardLinkAttribute.Name, this.BackLinkAttribute.Name);
            }
        }

        public AcmaSchemaObjectClass ForwardLinkObjectClass
        {
            get
            {
                return this.Model.ForwardLinkObjectClass;
            }
        }

        public string ForwardLinkObjectClassName
        {
            get
            {
                return this.ForwardLinkObjectClass == null ? null : this.ForwardLinkObjectClass.Name;
            }
        }

        public AcmaSchemaObjectClass BackLinkObjectClass
        {
            get
            {
                return this.Model.BackLinkObjectClass;
            }
        }

        public string BackLinkObjectClassName
        {
            get
            {
                return this.BackLinkObjectClass == null ? null : this.BackLinkObjectClass.Name;
            }
        }

        public AcmaSchemaAttribute BackLinkAttribute
        {
            get
            {
                return this.Model.BackLinkAttribute;
            }
        }

        public string BackLinkAttributeName
        {
            get
            {
                return this.BackLinkAttribute == null ? null : this.BackLinkAttribute.Name;
            }
        }

        public AcmaSchemaAttribute ForwardLinkAttribute
        {
            get
            {
                return this.Model.ForwardLinkAttribute;
            }
        }

        public string ForwardLinkAttributeName
        {
            get
            {
                return this.ForwardLinkAttribute == null ? null : this.ForwardLinkAttribute.Name;
            }
        }

        private bool CanDeleteLink()
        {
            return true;
        }

        private void DeleteLink()
        {
            try
            {
                if (MessageBox.Show("Are you are you want to delete this link?\nAny attribute values populated by this link will not be removed", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    this.parentVM.DeleteReferenceLink(this.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the mapping\n\n" + ex.Message);
            }
        }
    }
}
