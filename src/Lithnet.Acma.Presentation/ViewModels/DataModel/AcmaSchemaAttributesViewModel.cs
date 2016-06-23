using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaAttributesViewModel : ListViewModel<AcmaSchemaAttributeViewModel, AcmaSchemaAttribute>
    {
        private IBindingList model;

        public AcmaSchemaAttributesViewModel(IBindingList model)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(this.model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddAttribute", t => this.AddAttribute());

            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Attributes");
            }
        }

        public void AddAttribute()
        {
            NewAttributeWindow window = new NewAttributeWindow();
            NewAcmaSchemaAttributeViewModel vm = new NewAcmaSchemaAttributeViewModel(window, this);
            window.DataContext = vm;
            window.ShowDialog();
        }

        internal void CreateAttribute(string name, ExtendedAttributeType type, bool isMultivalued, AcmaAttributeOperation operation, bool isIndexable, bool isIndexed)
        {
            AcmaSchemaAttribute attribute;
            try
            {
                attribute = ActiveConfig.DB.CreateAttribute(name, type, isMultivalued, operation, isIndexable, isIndexed);

                this.Add(attribute);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the attribute\n\n" + ex.Message);
            }
        }

        private AcmaSchemaAttributeViewModel ViewModelResolver(AcmaSchemaAttribute model)
        {
            return new AcmaSchemaAttributeViewModel(model);
        }
    }
}