using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;
using Lithnet.Fim.Core;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class ValueMergeTransformViewModel : TransformViewModel
    {
        private ValueMergeTransform model;

        public ValueMergeTransformViewModel(ValueMergeTransform model)
            : base(model)
        {
            this.model = model;
        }

        public ExtendedAttributeType UserDefinedReturnType
        {
            get
            {
                return model.UserDefinedReturnType ;
            }
            set
            {
                model.UserDefinedReturnType = value;
            }
        }

        public IEnumerable<EnumExtension.EnumMember> AllowedTypes
        {
            get
            {
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.String.GetEnumDescription(), Value = ExtendedAttributeType.String };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Integer.GetEnumDescription(), Value = ExtendedAttributeType.Integer };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Boolean.GetEnumDescription(), Value = ExtendedAttributeType.Boolean };
                yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Binary.GetEnumDescription(), Value = ExtendedAttributeType.Binary };

                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return new EnumExtension.EnumMember() { Description = ExtendedAttributeType.DateTime.GetEnumDescription(), Value = ExtendedAttributeType.DateTime };
                }
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.ValueMergeTransformDescription;
            }
        }
    }
}
