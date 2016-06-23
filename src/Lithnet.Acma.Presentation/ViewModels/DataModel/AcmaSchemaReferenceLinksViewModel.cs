using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaReferenceLinksViewModel : ListViewModel<AcmaSchemaReferenceLinkViewModel, AcmaSchemaReferenceLink>
    {
        private AcmaSchemaObjectClass objectClass;

        private IBindingList model;

        public AcmaSchemaReferenceLinksViewModel(IBindingList model, AcmaSchemaObjectClass objectClass)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddLink", t => this.CreateLink());

            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.objectClass = objectClass;
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.objectClass;
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Reference back-links");
            }
        }

        public void CreateLink()
        {
            NewReferenceLinkWindow window = new NewReferenceLinkWindow();
            NewAcmaSchemaReferenceLinkViewModel vm = new NewAcmaSchemaReferenceLinkViewModel(window, this);
            window.DataContext = vm;
            window.ShowDialog();
        }

        public void AddReferenceLink(AcmaSchemaObjectClass forwardLinkObjectClass, AcmaSchemaAttribute forwardLinkAttribute, AcmaSchemaObjectClass backLinkObjectClass, AcmaSchemaAttribute backLinkAttribute)
        {
            this.Add(ActiveConfig.DB.CreateReferenceLink(objectClass, forwardLinkAttribute, backLinkObjectClass, backLinkAttribute, this.model));
        }

        public void DeleteReferenceLink(AcmaSchemaReferenceLink link)
        {
            ActiveConfig.DB.DeleteReferenceLink(link, this.model);
            this.Remove(link);
        }

        private AcmaSchemaReferenceLinkViewModel ViewModelResolver(AcmaSchemaReferenceLink model)
        {
            return new AcmaSchemaReferenceLinkViewModel(model, this);
        }
    }
}