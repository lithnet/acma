using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Fim.Core;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class MultivalueToSingleValueTransformViewModel : TransformViewModel
    {
        private MultivalueToSingleValueTransform model;

        private List<EnumExtension.EnumMember> allowedValueOperators;

        private List<EnumExtension.EnumMember> allowedCompareAsTypes;

        public MultivalueToSingleValueTransformViewModel(MultivalueToSingleValueTransform model)
            : base(model)
        {
            this.model = model;

            this.allowedCompareAsTypes = new List<EnumExtension.EnumMember>();

            this.allowedCompareAsTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.String.GetEnumDescription(), Value = ExtendedAttributeType.String });
            this.allowedCompareAsTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Integer.GetEnumDescription(), Value = ExtendedAttributeType.Integer });
            this.allowedCompareAsTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Boolean.GetEnumDescription(), Value = ExtendedAttributeType.Boolean });
            this.allowedCompareAsTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Reference.GetEnumDescription(), Value = ExtendedAttributeType.Reference });
            this.allowedCompareAsTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Binary.GetEnumDescription(), Value = ExtendedAttributeType.Binary });
                
            if (TransformGlobal.HostProcessSupportsNativeDateTime)
            {
               this.allowedCompareAsTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.DateTime.GetEnumDescription(), Value = ExtendedAttributeType.DateTime });
            }
        }

        public string SelectorValue
        {
            get
            {
                return model.SelectorValue;
            }
            set
            {
                model.SelectorValue = value;
            }
        }

        public ValueOperator SelectorOperator
        {
            get
            {
                return model.SelectorOperator;
            }
            set
            {
                model.SelectorOperator = value;
            }
        }

        [PropertyChanged.AlsoNotifyFor("AllowedValueOperators")]
        public ExtendedAttributeType CompareAs
        {
            get
            {
                return model.CompareAs;
            }
            set
            {
                model.CompareAs = value;
            }
        }

        public IEnumerable<EnumExtension.EnumMember> AllowedCompareAsTypes
        {
            get
            {
                return this.allowedCompareAsTypes;
            }
        }

        public IEnumerable<EnumExtension.EnumMember> AllowedValueOperators
        {
            get
            {
                if (this.allowedValueOperators == null)
                {
                    this.allowedValueOperators = new List<EnumExtension.EnumMember>();
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.Equals.GetEnumDescription(), Value = ValueOperator.Equals });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.NotEquals.GetEnumDescription(), Value = ValueOperator.NotEquals });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.GreaterThan.GetEnumDescription(), Value = ValueOperator.GreaterThan });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.GreaterThanOrEq.GetEnumDescription(), Value = ValueOperator.GreaterThanOrEq });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.LessThan.GetEnumDescription(), Value = ValueOperator.LessThan });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.LessThanOrEq.GetEnumDescription(), Value = ValueOperator.LessThanOrEq });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.Contains.GetEnumDescription(), Value = ValueOperator.Contains });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.NotContains.GetEnumDescription(), Value = ValueOperator.NotContains });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.StartsWith.GetEnumDescription(), Value = ValueOperator.StartsWith });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.EndsWith.GetEnumDescription(), Value = ValueOperator.EndsWith });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.Smallest.GetEnumDescription(), Value = ValueOperator.Smallest });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.Largest.GetEnumDescription(), Value = ValueOperator.Largest });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.First.GetEnumDescription(), Value = ValueOperator.First });
                    this.allowedValueOperators.Add(new EnumExtension.EnumMember() { Description = ValueOperator.Last.GetEnumDescription(), Value = ValueOperator.Last });
                }

                List<EnumExtension.EnumMember> workingList = new List<EnumExtension.EnumMember>();
                workingList.AddRange(this.allowedValueOperators.Where(t => ComparisonEngine.GetAllowedValueOperators(this.CompareAs).Contains((ValueOperator)t.Value)));
                workingList.Add(this.allowedValueOperators.First(t => (ValueOperator)t.Value == ValueOperator.First));
                workingList.Add(this.allowedValueOperators.First(t => (ValueOperator)t.Value == ValueOperator.Last));

                if (this.CompareAs == ExtendedAttributeType.Integer || this.CompareAs == ExtendedAttributeType.DateTime)
                {
                    workingList.Add(this.allowedValueOperators.First(t => (ValueOperator)t.Value == ValueOperator.Smallest));
                    workingList.Add(this.allowedValueOperators.First(t => (ValueOperator)t.Value == ValueOperator.Largest));
                }

                return workingList;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.MultivalueToSingleValueTransformDescription;
            }
        }
    }
}
