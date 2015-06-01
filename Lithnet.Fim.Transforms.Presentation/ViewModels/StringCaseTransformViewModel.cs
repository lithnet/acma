using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class StringCaseTransformViewModel : TransformViewModel
    {
        private StringCaseTransform model;

        public StringCaseTransformViewModel(StringCaseTransform model)
            : base(model)
        {
            this.model = model;
        }

        public StringCaseType StringCase
        {
            get
            {
                return model.StringCase;
            }
            set
            {
                model.StringCase = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.StringCaseTransformDescription;
            }
        }
    }
}
