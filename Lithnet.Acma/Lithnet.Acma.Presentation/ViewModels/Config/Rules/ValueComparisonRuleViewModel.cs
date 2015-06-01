using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;
using ICSharpCode.AvalonEdit.Document;

namespace Lithnet.Acma.Presentation
{
    public class ValueComparisonRuleViewModel : RuleViewModel
    {
        private ValueComparisonRule typedModel;

        private ValueDeclarationViewModel expectedValue;

        private static List<EnumExtension.EnumMember> valueOperatorValues = new List<EnumExtension.EnumMember>();

        public ValueComparisonRuleViewModel(ValueComparisonRule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;
            if (this.typedModel.ExpectedValue == null)
            {
                this.typedModel.ExpectedValue = new ValueDeclaration();
            }

            this.ExpectedValue = new ValueDeclarationViewModel(this.typedModel.ExpectedValue, this.typedModel.ObjectClass);
        }

        public GroupOperator GroupOperator
        {
            get
            {
                return this.typedModel.GroupOperator;
            }
            set
            {
                this.typedModel.GroupOperator = value;
            }
        }

        [PropertyChanged.DependsOn("Attribute", "ReferencedObjectAttribute")]
        public bool IsGroupOperatorEnabled
        {
            get
            {
                if (this.Attribute == null)
                {
                    return false;
                }
                else
                {
                    return this.Attribute.IsMultivalued || (this.ReferencedObjectAttribute != null && this.ReferencedObjectAttribute.IsMultivalued);
                }
            }
        }

        [PropertyChanged.DependsOn("IsReferenced", "IsLocal")]
        public IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                if (this.IsReferenced)
                {
                    return ActiveConfig.DB.Attributes.OrderBy(t => t.Name);
                }
                else
                {
                    return this.typedModel.ObjectClass.Attributes.OrderBy(t => t.Name);
                }
            }
        }

        [PropertyChanged.DependsOn("IsReferenced", "IsLocal")]
        public IEnumerable<AcmaSchemaAttribute> AllowedReferenceSourceAttributes
        {
            get
            {
                return this.typedModel.ObjectClass.Attributes.Where(t => t.Type == ExtendedAttributeType.Reference).OrderBy(t => t.Name);
            }
        }

        [PropertyChanged.DependsOn("IsReferenced", "IsLocal", "View", "Attribute", "Declaration", "GroupOperator", "ReferencedObjectAttribute", "ValueOperator")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName(true);
            }
        }

        private string GetDisplayName(bool truncate)
        {
            if (this.Attribute == null || (this.IsReferenced && this.ReferencedObjectAttribute == null))
            {
                return string.Format("Undefined attribute comparison rule");
            }
            else
            {
                string groupOperator = this.GroupOperator.GetEnumDescription().ToLower();
                string attributeName = this.IsReferenced ? this.ReferencedObjectAttribute.Name + "->" + this.Attribute.Name : this.Attribute.Name;
                string valueOperator = this.ValueOperator.GetEnumDescription().ToLower();

                string valueString;

                if (truncate)
                {
                    valueString = this.ExpectedValue.Declaration.TruncateString(40);
                }
                else
                {
                    valueString = this.ExpectedValue.Declaration;
                }

                if (this.View == HologramView.Current)
                {
                    attributeName = "#" + attributeName;
                }

                if (this.Attribute.IsMultivalued)
                {
                    return string.Format("If {0} values in {{{1}}} {2} {3}", groupOperator, attributeName, valueOperator, valueString);
                }
                else
                {
                    return string.Format("If {{{0}}} {1} {2}", attributeName, valueOperator, valueString);
                }
            }
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName(false);
            }
        }
        
        public ValueDeclarationViewModel ExpectedValue
        {
            get
            {
                return this.expectedValue;
            }

            set
            {
                if (this.expectedValue != null)
                {
                    this.expectedValue.PropertyChanged -= expectedValue_PropertyChanged;
                }

                this.expectedValue = value;

                if (this.expectedValue != null)
                {
                    this.expectedValue.PropertyChanged += expectedValue_PropertyChanged;
                }
            }
        }

        private void expectedValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Declaration")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }

        public ValueOperator ValueOperator
        {
            get
            {
                return this.typedModel.ValueOperator;
            }
            set
            {
                this.typedModel.ValueOperator = value;
            }
        }

        public IEnumerable<EnumExtension.EnumMember> AllowedValueOperators
        {
            get
            {
                if (this.Attribute == null)
                {
                    return null;
                }

                return this.GetAllowedValueOperators(this.Attribute.Type);
            }
        }

        private IEnumerable<EnumExtension.EnumMember> GetAllowedValueOperators(ExtendedAttributeType type)
        {
            if (ValueComparisonRuleViewModel.valueOperatorValues.Count == 0)
            {
                var enumValues = Enum.GetValues(typeof(ValueOperator));

                foreach (var value in enumValues)
                {
                    EnumExtension.EnumMember enumMember = new EnumExtension.EnumMember();
                    enumMember.Value = value;
                    enumMember.Description = ((Enum)value).GetEnumDescription();
                    valueOperatorValues.Add(enumMember);
                }
            }

            foreach (var value in ComparisonEngine.GetAllowedValueOperators(type))
            {
                yield return valueOperatorValues.FirstOrDefault(t => (int)t.Value == ((int)value));
            }
        }

        public HologramView View
        {
            get
            {
                return this.typedModel.View;
            }
            set
            {
                this.typedModel.View = value;
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

        public AcmaSchemaAttribute ReferencedObjectAttribute
        {
            get
            {
                return this.typedModel.ReferencedObjectAttribute;
            }
            set
            {
                this.typedModel.ReferencedObjectAttribute = value;
            }
        }

        [PropertyChanged.AlsoNotifyFor("IsLocal")]
        public bool IsReferenced
        {
            get
            {
                return this.typedModel.IsReferenced;
            }
            set
            {
                if (this.typedModel.IsReferenced == value)
                {
                    return;
                }

                this.typedModel.IsReferenced = value;

                if (value)
                {
                    this.View = HologramView.Current;
                }
                else
                {
                    this.ReferencedObjectAttribute = null;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("IsReferenced")]
        public bool IsLocal
        {
            get
            {
                return !this.IsReferenced;
            }
            set
            {
                if (this.typedModel.IsReferenced != value)
                {
                    return;
                }

                this.IsReferenced = !value;
            }
        }

        public bool CanSetView
        {
            get
            {
                return this.IsLocal && this.CanUseProposedValues;
            }
        }
    }
}
