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
    public class NewAcmaSchemaReferenceLinkViewModel : ViewModelBase
    {
        private AcmaSchemaReferenceLinksViewModel referenceLinksViewModel;

        public NewAcmaSchemaReferenceLinkViewModel(Window window, AcmaSchemaReferenceLinksViewModel referenceLinksViewModel)
            : base()
        {
            this.referenceLinksViewModel = referenceLinksViewModel;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                return "Create back-link";
            }
        }

        public AcmaSchemaObjectClass ForwardLinkObjectClass
        {
            get
            {
                return this.referenceLinksViewModel.ObjectClass;
            }
        }

        public IEnumerable<AcmaSchemaAttribute> AllowedForwardLinkAttributes
        {
            get
            {
                return this.ForwardLinkObjectClass.Attributes.Where(t => t.IsValidForwardLink(this.ForwardLinkObjectClass)).OrderBy(t => t.Name);
            }
        }

        public IEnumerable<AcmaSchemaObjectClass> AllowedBackLinkObjectClasses
        {
            get
            {
                return ActiveConfig.DB.ObjectClasses.OrderBy(t => t.Name);
            }
        }

        public AcmaSchemaAttribute ForwardLinkAttribute { get; set; }

        public AcmaSchemaObjectClass BackLinkObjectClass { get; set; }

        public AcmaSchemaAttribute BackLinkAttribute { get; set; }

        public IEnumerable<AcmaSchemaAttribute> AllowedBackLinkAttributes
        {
            get
            {
                if (this.BackLinkObjectClass == null)
                {
                    return null;
                }

                IEnumerable<AcmaSchemaAttribute> attributes = this.BackLinkObjectClass.Attributes.Where(t => t.IsValidBackLink(this.BackLinkObjectClass));

                if (this.BackLinkObjectClass == this.ForwardLinkObjectClass)
                {
                    return attributes.Except(new List<AcmaSchemaAttribute>() { this.ForwardLinkAttribute }).OrderBy(t => t.Name);
                }
                else
                {
                    return attributes.OrderBy(t => t.Name);
                }
            }
        }

        private bool CanCreate()
        {
            return true;
        }

        private void Create(Window window)
        {
            try
            {
                this.referenceLinksViewModel.AddReferenceLink(this.ForwardLinkObjectClass, this.ForwardLinkAttribute, this.BackLinkObjectClass, this.BackLinkAttribute);
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the link\n\n" + ex.Message);
            }
        }
    }
}
