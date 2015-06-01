using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class ADGroupTypeToStringTransformViewModel : TransformViewModel
    {
        private ADGroupTypeToStringTransform model;

        public ADGroupTypeToStringTransformViewModel(ADGroupTypeToStringTransform model)
            : base(model)
        {
            this.model = model;
        }


        public override string TransformDescription
        {
            get
            {
                return strings.ADGroupTypeToStringTransformDescription;
            }
        }
    }
}
