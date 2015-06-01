using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaObjectViewModel : ViewModelBase<AcmaSchemaObjectClass>
    {
        private AcmaSchemaObjectClass model;

        private AcmaSchemaMappingsViewModel mappingsVM;

        private AcmaSchemaShadowObjectLinksViewModel shadowLinksVM;

        private AcmaSchemaReferenceLinksViewModel referenceLinksVM;

        private AcmaSchemaObjectsViewModel parentVM;

        public AcmaSchemaObjectViewModel(AcmaSchemaObjectClass model, AcmaSchemaObjectsViewModel parentVM)
            : base(model)
        {
            this.Commands.AddItem("DeleteObjectClass", t => this.DeleteObjectClass());
            this.model = model;
            this.parentVM = parentVM;

            this.Mappings = new AcmaSchemaMappingsViewModel(this.model.MappingsBindingList, this.model);
            this.ShadowLinks = new AcmaSchemaShadowObjectLinksViewModel(this.model.ShadowLinksBindingList, this.model);
            this.ReferenceLinks = new AcmaSchemaReferenceLinksViewModel(this.model.ReferenceLinksBindingList, this.model);

            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0}{1}", this.Name, string.IsNullOrWhiteSpace(this.ShadowFromClass) ? string.Empty : string.Format(" (<-{0})", this.ShadowFromClass));
            }
        }

        public string Name
        {
            get
            {
                return this.model.Name;
            }
            set
            {
                if (value != this.model.Name)
                {
                    ActiveConfig.DB.RenameObjectClass(this.model.Name, value);
                    ActiveConfig.DB.RefreshEntity(this.model);
                }
            }
        }

        public string ShadowFromClass
        {
            get
            {
                return this.model.IsShadowObject ? this.model.ShadowFromObjectClass.Name : null;
            }
        }

        public bool AllowResurrection
        {
            get
            {
                return this.model.AllowResurrection;
            }
            set
            {
                this.model.AllowResurrection = value;
                ActiveConfig.DB.Commit();
            }
        }

        public IEnumerable ChildNodes
        {
            get
            {
                yield return this.Mappings;
                yield return this.ReferenceLinks;

                if (this.IsShadowObject)
                {
                    yield return this.ShadowLinks;
                }
            }
        }

        public bool IsShadowParent
        {
            get
            {
                return this.model.IsShadowParent;
            }
        }

        public bool IsShadowObject
        {
            get
            {
                return this.model.IsShadowObject;
            }
        }

        public AcmaSchemaMappingsViewModel Mappings
        {
            get
            {
                return this.mappingsVM;
            }
            set
            {
                if (this.mappingsVM != null)
                {
                    this.UnregisterChildViewModel(this.mappingsVM);
                }

                this.mappingsVM = value;

                this.RegisterChildViewModel(this.mappingsVM);
            }
        }

        public AcmaSchemaShadowObjectLinksViewModel ShadowLinks
        {
            get
            {
                return this.shadowLinksVM;
            }
            set
            {
                if (this.shadowLinksVM != null)
                {
                    this.UnregisterChildViewModel(this.shadowLinksVM);
                }

                this.shadowLinksVM = value;

                this.RegisterChildViewModel(this.shadowLinksVM);
            }
        }

        public AcmaSchemaReferenceLinksViewModel ReferenceLinks
        {
            get
            {
                return this.referenceLinksVM;
            }
            set
            {
                if (this.referenceLinksVM != null)
                {
                    this.UnregisterChildViewModel(this.referenceLinksVM);
                }

                this.referenceLinksVM = value;

                this.RegisterChildViewModel(this.referenceLinksVM);
            }
        }

        private void DeleteObjectClass()
        {
            this.parentVM.DeleteObjectClass(this.model);
        }
    }
}
