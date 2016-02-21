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
    public class NewAcmaSchemaMappingViewModel : ViewModelBase
    {
        private AcmaSchemaMappingsViewModel mappingsViewModel;

        public NewAcmaSchemaMappingViewModel(Window window, AcmaSchemaMappingsViewModel mappingsViewModel)
            : base()
        {
            this.mappingsViewModel = mappingsViewModel;
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.Commands.AddItem("CreateAndAddNew", t => this.CreateAndAddNew(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                return "Create New Binding";
            }
        }

        public IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                if (this.IsInherited)
                {
                    return ActiveConfig.DB.Attributes
                        .Where(t => !t.IsBuiltIn && t.CanInherit && (t != this.InheritanceSource))
                        .Except(this.mappingsViewModel.ObjectClass.Attributes)
                        .OrderBy(t => t.Name);
                }
                else
                {
                    return ActiveConfig.DB.Attributes
                        .Where(t => !t.IsBuiltIn && (t != this.InheritanceSource))
                        .Except(this.mappingsViewModel.ObjectClass.Attributes)
                        .OrderBy(t => t.Name);
                }
            }
        }

        public AcmaSchemaAttribute Attribute { get; set; }

        public string ObjectClassName
        {
            get
            {
                return this.mappingsViewModel.ObjectClass.Name;
            }
        }

        public AcmaSchemaAttribute InheritanceSource { get; set; }

        public IEnumerable<AcmaSchemaAttribute> AllowedInheritanceSourceAttributes
        {
            get
            {
                return this.mappingsViewModel.ObjectClass.Attributes.Where(t => t.IsInheritanceSourceCandidate).Except(
                   new List<AcmaSchemaAttribute>() { this.InheritedAttribute, this.Attribute }).OrderBy(t => t.Name);
            }
        }

        public AcmaSchemaAttribute InheritedAttribute { get; set; }

        public AcmaSchemaObjectClass InheritanceSourceObject { get; set; }

        public IEnumerable<AcmaSchemaObjectClass> AllowedInheritanceSourceObjects
        {
            get
            {
                return ActiveConfig.DB.ObjectClasses;
            }
        }

        public IEnumerable<AcmaSchemaAttribute> AllowedInheritedAttributes
        {
            get
            {
                if (this.Attribute == null || this.InheritanceSourceObject == null)
                {
                    return null;
                }
                else
                {
                    return this.InheritanceSourceObject.Attributes.Where(t => t.Mappings.Count > 0 && t.IsInheritable && t.Type == this.Attribute.Type).OrderBy(t => t.Name);
                }
            }
        }

        public bool IsInherited { get; set; }

        private bool CanCreate()
        {
            if (this.IsInherited)
            {
                if (this.InheritanceSource == null || this.InheritedAttribute == null || this.InheritanceSourceObject == null)
                {
                    return false;
                }

                if (this.Attribute == this.InheritanceSource)
                {
                    return false;
                }

                if (this.InheritedAttribute == this.InheritanceSource)
                {
                    return false;
                }
            }

            if (this.Attribute == null)
            {
                return false;
            }


            return true;
        }

        private void Create(Window window, bool close = true)
        {
            try
            {
                if (this.IsInherited)
                {
                    this.mappingsViewModel.AddMapping(this.mappingsViewModel.ObjectClass, this.Attribute, this.InheritanceSource, this.InheritanceSourceObject, this.InheritedAttribute);
                }
                else
                {
                    this.mappingsViewModel.AddMapping(this.mappingsViewModel.ObjectClass, this.Attribute);
                }

                if (close)
                {
                    window.Close();
                }
                else
                {
                    this.RaisePropertyChanged("AllowedInheritanceSourceAttributes");
                    this.RaisePropertyChanged("AllowedAttributes");
                    this.RaisePropertyChanged("AllowedInheritedAttributes");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the attribute\n\n" + ex.Message);
            }
        }

        private void CreateAndAddNew(Window window)
        {
            this.Create(window, false);
        }
    }
}
