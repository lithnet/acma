using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;
using Lithnet.Fim.Core;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class TypeConverterTransformViewModel : TransformViewModel
    {
        private TypeConverterTransform model;

        public TypeConverterTransformViewModel(TypeConverterTransform model)
            : base(model)
        {
            this.model = model;
        }

        public ExtendedAttributeType ConvertToType
        {
            get
            {
                return model.ConvertToType;
            }
            set
            {
                model.ConvertToType = value;
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
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.TypeConverterTransformDescription;
            }
        }
    }
}
