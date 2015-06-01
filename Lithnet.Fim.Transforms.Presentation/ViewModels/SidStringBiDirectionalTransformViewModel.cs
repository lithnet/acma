using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class SidStringBiDirectionalTransformViewModel : TransformViewModel
    {
        private SidStringBiDirectionalTransform model;

        public SidStringBiDirectionalTransformViewModel(SidStringBiDirectionalTransform model)
            : base(model)
        {
            this.model = model;
        }


        public override string TransformDescription
        {
            get
            {
                return strings.SidStringBiDirectionalTransformDesciption;
            }
        }
    }
}
