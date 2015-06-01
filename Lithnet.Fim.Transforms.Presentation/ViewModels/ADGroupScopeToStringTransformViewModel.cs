using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class ADGroupScopeToStringTransformViewModel : TransformViewModel
    {
        private ADGroupScopeToStringTransform model;

        public ADGroupScopeToStringTransformViewModel(ADGroupScopeToStringTransform model)
            : base(model)
        {
            this.model = model;
        }


        public override string TransformDescription
        {
            get
            {
                return strings.ADGroupScopeToStringTransformDescription;
            }
        }
    }
}
