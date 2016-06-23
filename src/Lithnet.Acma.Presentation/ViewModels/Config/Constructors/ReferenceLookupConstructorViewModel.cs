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
    public class ReferenceLookupConstructorViewModel : AttributeConstructorViewModel
    {
        private ReferenceLookupConstructor typedModel;

        private DBQueryGroupViewModel queryGroup;

        private List<EnumExtension.EnumMember> allowedActions;

        public ReferenceLookupConstructorViewModel(ReferenceLookupConstructor model)
            : base(model)
        {
            this.typedModel = model;
            this.QueryGroup = new DBQueryGroupViewModel(this.typedModel.QueryGroup, "Lookup query");
        }

        public override IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                return this.Model.ObjectClass.Attributes.Where(t => !t.IsReadOnlyInClass(this.Model.ObjectClass) && t.Type == ExtendedAttributeType.Reference).OrderBy(t => t.Name);
            }
        }

        public MultipleResultAction MultipleResultAction
        {
            get
            {
                return this.typedModel.MultipleResultAction;
            }
            set
            {
                this.typedModel.MultipleResultAction = value;
            }
        }

        public IEnumerable<EnumExtension.EnumMember> AllowedActions
        {
            get
            {
                if (this.allowedActions == null)
                {
                    this.allowedActions = new List<EnumExtension.EnumMember>();
                    this.allowedActions.Add(new EnumExtension.EnumMember() { Value = MultipleResultAction.UseAll, Description = MultipleResultAction.UseAll.GetEnumDescription() });
                    this.allowedActions.Add(new EnumExtension.EnumMember() { Value = MultipleResultAction.UseFirst, Description = MultipleResultAction.UseFirst.GetEnumDescription() });
                    this.allowedActions.Add(new EnumExtension.EnumMember() { Value = MultipleResultAction.UseNone, Description = MultipleResultAction.UseNone.GetEnumDescription() });
                    this.allowedActions.Add(new EnumExtension.EnumMember() { Value = MultipleResultAction.Error, Description = MultipleResultAction.Error.GetEnumDescription() });
                
                }

                if (this.Attribute == null)
                {
                    return null;
                }

                if (this.Attribute.IsMultivalued)
                {
                    return this.allowedActions;
                }
                else
                {
                    return this.allowedActions.Where(t => (MultipleResultAction)t.Value != MultipleResultAction.UseAll);
                }
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Attribute")
            {
                this.RaisePropertyChanged("AllowedActions");
            }
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                if (this.RuleGroup != null)
                {
                    yield return this.RuleGroup;
                }

                if (this.QueryGroup != null)
                {
                    yield return this.QueryGroup;
                }
            }
        }

        public DBQueryGroupViewModel QueryGroup
        {
            get
            {
                return this.queryGroup;
            }
            set
            {
                if (this.queryGroup != null)
                {
                    this.UnregisterChildViewModel(this.queryGroup);
                }

                this.queryGroup = value;
                this.RegisterChildViewModel(this.queryGroup);
            }
        }
    }
}
