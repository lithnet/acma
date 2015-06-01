using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class TrimStringTransformViewModel : TransformViewModel
    {
        private TrimStringTransform model;

        public TrimStringTransformViewModel(TrimStringTransform model)
            : base(model)
        {
            this.model = model;
        }

        public TrimType TrimType
        {
            get
            {
                return model.TrimType;
            }
            set
            {
                model.TrimType = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.TrimStringTransformDescription;
            }
        }
    }
}
