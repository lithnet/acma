using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class NewAcmaSchemaShadowObjectLinkViewModel : ViewModelBase
    {
        private AcmaSchemaShadowObjectLinksViewModel linksViewModel;

        private AcmaSchemaObjectClass shadowParentObjectClass;

        public NewAcmaSchemaShadowObjectLinkViewModel(Window window, AcmaSchemaShadowObjectLinksViewModel linksViewModel)
            : base()
        {
            this.linksViewModel = linksViewModel;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.shadowParentObjectClass = linksViewModel.ObjectClass.ShadowFromObjectClass;
        }

        public string DisplayName
        {
            get
            {
                return "Create New Shadow Object Link";
            }
        }

        public string ParentObjectClassName
        {
            get
            {
                return this.linksViewModel.ObjectClass.ShadowFromObjectClass.Name;
            }
        }

        public IEnumerable<AcmaSchemaAttribute> AllowedReferenceAttributes
        {
            get
            {
                return this.shadowParentObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Reference && !t.IsMultivalued).Except(
                    this.shadowParentObjectClass.ShadowChildLinks.Select(t => t.ReferenceAttribute)).OrderBy(t => t.Name);
            }
        }

        public AcmaSchemaAttribute ReferenceAttribute { get; set; }

        public AcmaSchemaAttribute ProvisioningAttribute { get; set; }

        public string Name { get; set; }

        public IEnumerable<AcmaSchemaAttribute> AllowedProvisioningAttributes
        {
            get
            {
                return this.shadowParentObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Boolean).Except(
                   this.shadowParentObjectClass.ShadowChildLinks.Select(t => t.ProvisioningAttribute)).OrderBy(t => t.Name);
            }
        }
        
        private bool CanCreate()
        {
            if (this.ProvisioningAttribute  == null || this.ReferenceAttribute == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return false;
            }

            return true;
        }

        private void Create(Window window)
        {
            try
            {
                this.linksViewModel.AddLink(this.ProvisioningAttribute, this.ReferenceAttribute, this.Name);

                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the attribute\n\n" + ex.Message);
            }
        }
    }
}
