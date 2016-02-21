using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public abstract class AttributeConstructorViewModel : ExecutableConstructorObjectViewModel
    {
        private AttributeConstructor typedModel;

        public AttributeConstructorViewModel(AttributeConstructor model)
            : base(model)
        {
            this.Commands.AddItem("DeleteConstructor", t => this.DeleteConstructor());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.typedModel = model;
        }

        public override string DisplayName
        {
            get
            {
                if (this.Attribute != null)
                {
                    return string.Format("{0} {{{1}}}", this.Id, this.Attribute.Name);

                }
                else
                {
                    if (this.Id == null)
                    {
                        return "Undefined constructor";
                    }
                    else
                    {
                        return string.Format("{0}", this.Id);
                    }
                }
            }
        }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public AcmaSchemaAttribute Attribute
        {
            get
            {
                return this.typedModel.Attribute;
            }
            set
            {
                this.typedModel.Attribute = value;
            }
        }

        public virtual IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                return this.Model.ObjectClass.Attributes.Where(t => !t.IsReadOnlyInClass(this.Model.ObjectClass)).OrderBy(t => t.Name);
            }
        }

        public string Type
        {
            get
            {

                return this.Model.GetType().GetTypeDescription();
            }
        }

        public override bool CanCopy()
        {
            return true;
        }

        public override bool CanCut()
        {
            return true;
        }

        private void DeleteConstructor()
        {
            this.ParentCollection.Remove(this.Model);
        }
    }
}
