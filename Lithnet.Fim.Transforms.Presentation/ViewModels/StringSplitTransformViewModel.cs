using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Fim.Core;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class StringSplitTransformViewModel : TransformViewModel
    {
        private StringSplitTransform model;

        public StringSplitTransformViewModel(StringSplitTransform model)
            : base(model)
        {
            this.model = model;
        }

        public string SplitRegex
        {
            get
            {
                return model.SplitRegex;
            }
            set
            {
                model.SplitRegex = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.StringSplitTransformDescription;
            }
        }
    }
}
