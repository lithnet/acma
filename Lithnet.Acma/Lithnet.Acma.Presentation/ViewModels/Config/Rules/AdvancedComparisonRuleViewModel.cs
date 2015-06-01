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
    public class AdvancedComparisonRuleViewModel : RuleViewModel
    {
        private AdvancedComparisonRule typedModel;

        private ValueDeclarationViewModel sourceValue;

        private ValueDeclarationViewModel targetValue;

        private static List<EnumExtension.EnumMember> valueOperatorValues = new List<EnumExtension.EnumMember>();

        public AdvancedComparisonRuleViewModel(AdvancedComparisonRule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.typedModel = model;

            if (this.typedModel.SourceValue == null)
            {
                this.typedModel.SourceValue = new ValueDeclaration();
            }

            if (this.typedModel.TargetValue == null)
            {
                this.typedModel.TargetValue = new ValueDeclaration();
            }

            this.SourceValue = new ValueDeclarationViewModel(this.typedModel.SourceValue, this.typedModel.ObjectClass);
            this.TargetValue = new ValueDeclarationViewModel(this.typedModel.TargetValue, this.typedModel.ObjectClass);
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

        [PropertyChanged.DependsOn("GroupOperator", "ValueOperator", "TargetValue", "SourceValue")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName(true);
            }
        }

        private string GetDisplayName(bool truncate)
        {
            string groupOperator = this.GroupOperator.GetEnumDescription().ToLower();
            string left = "\"" + this.SourceValue.Declaration + "\"" + (string.IsNullOrWhiteSpace(this.SourceValue.TransformsString) ? string.Empty : ">>" + this.SourceValue.TransformsString);
            string right = "\"" + this.TargetValue.Declaration + "\"" + (string.IsNullOrWhiteSpace(this.TargetValue.TransformsString) ? string.Empty : ">>" + this.TargetValue.TransformsString);

            if (truncate)
            {
                right = right.TruncateString(40);
            }

            string valueOperator = this.ValueOperator.GetEnumDescription().ToLower();

            if (this.ValueOperator == Fim.Core.ValueOperator.IsPresent || this.ValueOperator == Fim.Core.ValueOperator.NotPresent)
            {
                return string.Format("If {0} values in {1} {2}", groupOperator, left, valueOperator);
            }
            else
            {
                return string.Format("If {0} values in {1} {2} {3}", groupOperator, left, valueOperator, right);
            }
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName(false);
            }
        }

        public ValueDeclarationViewModel TargetValue
        {
            get
            {
                return this.targetValue;
            }

            set
            {
                if (this.targetValue != null)
                {
                    this.UnregisterChildViewModel(this.targetValue);
                    this.targetValue.PropertyChanged -= expectedValue_PropertyChanged;
                }

                this.targetValue = value;

                if (this.targetValue != null)
                {
                    this.RegisterChildViewModel(this.targetValue);
                    this.targetValue.PropertyChanged += expectedValue_PropertyChanged;
                }
            }
        }

        public ValueDeclarationViewModel SourceValue
        {
            get
            {
                return this.sourceValue;
            }

            set
            {
                if (this.sourceValue != null)
                {
                    this.UnregisterChildViewModel(this.sourceValue);
                    this.sourceValue.PropertyChanged -= expectedValue_PropertyChanged;
                }

                this.sourceValue = value;

                if (this.sourceValue != null)
                {
                    this.RegisterChildViewModel(this.sourceValue);
                    this.sourceValue.PropertyChanged += expectedValue_PropertyChanged;
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
                return this.GetAllowedValueOperators(this.CompareAs);
            }
        }

        private IEnumerable<EnumExtension.EnumMember> GetAllowedValueOperators(ExtendedAttributeType type)
        {
            if (AdvancedComparisonRuleViewModel.valueOperatorValues.Count == 0)
            {
                var enumValues = Enum.GetValues(typeof(ValueOperator));
                EnumExtension.EnumMember enumMember;

                foreach (var value in enumValues)
                {
                     enumMember = new EnumExtension.EnumMember();
                    enumMember.Value = value;
                    enumMember.Description = ((Enum)value).GetEnumDescription();
                    valueOperatorValues.Add(enumMember);
                }

                enumMember = new EnumExtension.EnumMember();
                enumMember.Value = ValueOperator.IsPresent;
                enumMember.Description = ((Enum)enumMember.Value).GetEnumDescription();
                valueOperatorValues.Add(enumMember);

                enumMember = new EnumExtension.EnumMember();
                enumMember.Value = ValueOperator.NotPresent;
                enumMember.Description = ((Enum)enumMember.Value).GetEnumDescription();
                valueOperatorValues.Add(enumMember);
            }

            foreach (var value in ComparisonEngine.GetAllowedValueOperators(type))
            {
                yield return valueOperatorValues.FirstOrDefault(t => (int)t.Value == ((int)value));
            }

            yield return valueOperatorValues.FirstOrDefault(t => (ValueOperator)t.Value == ValueOperator.IsPresent);
            yield return valueOperatorValues.FirstOrDefault(t => (ValueOperator)t.Value == ValueOperator.NotPresent);
        }

        public bool IsSecondValueAllowed
        {
            get
            {
                return this.ValueOperator != Fim.Core.ValueOperator.NotPresent && this.ValueOperator != Fim.Core.ValueOperator.IsPresent;
            }
        }

        public ExtendedAttributeType CompareAs
        {
            get
            {
                return this.typedModel.CompareAs;
            }
            set
            {
                this.typedModel.CompareAs = value;
            }
        }
    }
}
