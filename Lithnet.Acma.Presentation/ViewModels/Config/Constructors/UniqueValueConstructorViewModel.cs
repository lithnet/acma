using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class UniqueValueConstructorViewModel : AttributeConstructorViewModel
    {
        UniqueValueConstructor typedModel;

        private AcmaSchemaAttributesViewModel allowedUniqueAllocationAttributes;

        private ValueDeclarationsViewModel valueDeclarations;

        public UniqueValueConstructorViewModel(UniqueValueConstructor model)
            : base(model)
        {
            this.Commands.AddItem("Add", t => this.AddAttribute(), t => this.CanAddAttribute());
            this.Commands.AddItem("Remove", t => this.RemoveAttribute(), t => this.CanRemoveAttribute());
            this.typedModel = model;
            this.UniqueAllocationAttributes = new AcmaSchemaAttributesViewModel(new BindingList<AcmaSchemaAttribute>(this.typedModel.UniqueAllocationAttributes));

            this.typedModel = model;
            if (this.typedModel.ValueDeclaration == null)
            {
                this.typedModel.ValueDeclaration = new UniqueValueDeclaration(null);
            }

            this.RegisterChildViewModel(this.typedModel.ValueDeclaration);

            if (this.typedModel.StaticDeclarations == null)
            {
                this.typedModel.StaticDeclarations = new List<ValueDeclaration>();
            }

            this.StaticDeclarations = new ValueDeclarationsViewModel(this.typedModel.StaticDeclarations, model.ObjectClass);

            if (this.StaticDeclarations.Count == 0)
            {
                this.StaticDeclarations.Add(new ValueDeclaration());
            }

            this.ValueDeclarationBindingList = this.StaticDeclarations.GetNewBindingList();
        }

        public string DeclarationString
        {
            get
            {
                return this.typedModel.ValueDeclarationString;
            }
            set
            {
                this.typedModel.ValueDeclarationString = value;
            }
        }

        public string TransformsString
        {
            get
            {
                return this.typedModel.ValueDeclaration.TransformsString;
            }
            set
            {
                this.typedModel.ValueDeclaration.TransformsString = value;
            }
        }

        private bool CanAddAttribute()
        {
            return this.AllowedUniqueAllocationAttributes.Any(t => t.IsSelected);
        }

        private void AddAttribute()
        {
            AcmaSchemaAttributeViewModel vm = this.AllowedUniqueAllocationAttributes.FirstOrDefault(t => t.IsSelected);

            if (vm != null)
            {
                this.UniqueAllocationAttributes.Add(vm.Model);
                this.AllowedUniqueAllocationAttributes.Remove(vm.Model);
            }
        }

        public override IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                return this.Model.ObjectClass.Attributes.Where(t => !t.IsReadOnlyInClass(this.Model.ObjectClass) && !t.IsMultivalued && t.Type == ExtendedAttributeType.String).OrderBy(t => t.Name);
            }
        }

        private bool CanRemoveAttribute()
        {
            return this.UniqueAllocationAttributes.Any(t => t.IsSelected);
        }

        private void RemoveAttribute()
        {
            AcmaSchemaAttributeViewModel vm = this.UniqueAllocationAttributes.FirstOrDefault(t => t.IsSelected);

            if (vm != null)
            {
                this.UniqueAllocationAttributes.Remove(vm.Model);
                this.AllowedUniqueAllocationAttributes.Add(vm.Model);
            }
        }

        private void Delete()
        {
            this.ParentCollection.Remove(this.Model);
        }

        public AcmaSchemaAttributesViewModel UniqueAllocationAttributes { get; set; }

        public AcmaSchemaAttributesViewModel AllowedUniqueAllocationAttributes
        {
            get
            {
                if (this.allowedUniqueAllocationAttributes == null)
                {
                    this.allowedUniqueAllocationAttributes = new AcmaSchemaAttributesViewModel(new BindingList<AcmaSchemaAttribute>(this.typedModel.ObjectClass.Attributes.Where(t =>
                  t.Type == ExtendedAttributeType.String
                  ).OrderBy(t => t.Name).ToList()));
                }

                return this.allowedUniqueAllocationAttributes;
            }
        }

        public ValueDeclarationsViewModel StaticDeclarations
        {
            get
            {
                return this.valueDeclarations;
            }
            set
            {
                if (this.valueDeclarations != null)
                {
                    this.UnregisterChildViewModel(this.valueDeclarations);
                }

                this.valueDeclarations = value;

                if (this.valueDeclarations != null)
                {
                    this.RegisterChildViewModel(this.valueDeclarations);
                }
            }
        }

        public ObservableCollection<ValueDeclarationViewModel> ValueDeclarationBindingList { get; private set; }


        public bool IsMissingIndex
        {
            get
            {
                return this.UniqueAllocationAttributes.Any(t => !t.IsIndexed);
            }
        }

        public string MissingIndexMessage
        {
            get
            {
                if (this.IsMissingIndex)
                {
                    return string.Format("The following attributes are not indexed, which may impact performance: {0}", 
                        this.UniqueAllocationAttributes.Where(t => !t.IsIndexed).Select(t => t.Name).ToCommaSeparatedString());
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
