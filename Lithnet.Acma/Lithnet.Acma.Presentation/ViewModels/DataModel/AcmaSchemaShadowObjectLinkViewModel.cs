using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaShadowObjectLinkViewModel : ViewModelBase<AcmaSchemaShadowObjectLink>
    {
        private AcmaSchemaShadowObjectLinksViewModel parentVM;

        public AcmaSchemaShadowObjectLinkViewModel(AcmaSchemaShadowObjectLink model, AcmaSchemaShadowObjectLinksViewModel parentVM)
            : base(model)
        {
            this.Commands.AddItem("DeleteLink", t => this.DeleteLink(), t => this.CanDeleteLink());
            this.Commands.AddItem("AddLink", t => this.parentVM.CreateLink());
            this.parentVM = parentVM;
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format(
                    "{0}",
                    this.ReferenceAttributeName
                    );
            }
        }

        public string ShadowObjectClassName
        {
            get
            {
                return this.Model.ShadowObjectClass.Name;
            }
        }

        public string ParentObjectClassName
        {
            get
            {
                return this.Model.ParentObjectClass.Name;
            }
        }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public string ProvisioningAttributeName
        {
            get
            {
                return this.Model.ProvisioningAttribute == null ? null : this.Model.ProvisioningAttribute.Name;
            }

        }

        public string ReferenceAttributeName
        {
            get
            {
                return this.Model.ReferenceAttribute == null ? null : this.Model.ReferenceAttribute.Name;
            }

        }

        private bool CanDeleteLink()
        {
            return true;
        }

        private void DeleteLink()
        {
            this.parentVM.DeleteLink(this.Model);
        }
    }
}
