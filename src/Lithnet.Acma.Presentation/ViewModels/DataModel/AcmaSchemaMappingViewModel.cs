using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaMappingViewModel : ViewModelBase<AcmaSchemaMapping>
    {
        private AcmaSchemaMappingsViewModel parentVM;

        public AcmaSchemaMappingViewModel(AcmaSchemaMapping model, AcmaSchemaMappingsViewModel parentVM)
            : base(model)
        {
            this.Commands.AddItem("DeleteMapping", t => this.DeleteMapping(), t => this.CanDeleteMapping());
            this.Commands.AddItem("AddMapping", t => this.parentVM.CreateMapping());
            //this.Commands.AddItem("ShowUsage", t => this.ShowUsage());
            this.parentVM = parentVM;
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format(
                    "{0}{1}",
                    this.AttributeName,
                    string.IsNullOrWhiteSpace(this.InheritanceSourceName) ? string.Empty : string.Format(" ({0}->{1})", this.InheritanceSourceName, this.InheritedAttributeName));
            }
        }

        public string AttributeName
        {
            get
            {
                return this.Model.Attribute.Name;
            }

        }

        public bool IsBuiltIn
        {
            get
            {
                return this.Model.IsBuiltIn;
            }
        }

        public bool IsInherited
        {
            get
            {
                return this.Model.InheritanceSourceAttribute != null;
            }
        }

        public string ObjectClassName
        {
            get
            {
                return this.Model.ObjectClass.Name;
            }

        }

        public string InheritanceSourceName
        {
            get
            {
                return this.Model.InheritanceSourceAttribute == null ? null : this.Model.InheritanceSourceAttribute.Name;
            }

        }

        public string InheritanceSourceObjectClassName
        {
            get
            {
                return this.Model.InheritanceSourceObjectClass == null ? null : this.Model.InheritanceSourceObjectClass.Name;
            }
        }

        public string InheritedAttributeName
        {
            get
            {
                return this.Model.InheritedAttribute == null ? null : this.Model.InheritedAttribute.Name;
            }
        }

        public IEnumerable<SchemaAttributeUsageViewModel> Usage
        {
            get
            {
                return ActiveConfig.XmlConfig.GetAttributeUsage(this.Model.Attribute, this.Model.ObjectClass).Select(t => new SchemaAttributeUsageViewModel(t, this));
            }
        }

        private void ShowUsage()
        {
            IEnumerable<SchemaAttributeUsage> usage = ActiveConfig.XmlConfig.GetAttributeUsage(this.Model.Attribute, this.Model.ObjectClass);

            if (usage != null && usage.Count() > 0)
            {
                MessageBox.Show(usage.Select(t => string.Format("{0}, {1}, {2}", t.ObjectType, t.Path, t.Context)).ToNewLineSeparatedString());
            }
        }


        private bool CanDeleteMapping()
        {
            return !this.IsBuiltIn;
        }

        private void DeleteMapping()
        {
            this.parentVM.DeleteMapping(this.Model);
        }
    }
}
