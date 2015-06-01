using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class EncodeStringTransformViewModel : TransformViewModel
    {
        private EncodeStringTransform model;

        public EncodeStringTransformViewModel(EncodeStringTransform model)
            : base(model)
        {
            this.model = model;
        }

        public StringEncodeFormat EncodeFormat
        {
            get
            {
                return model.EncodeFormat;
            }
            set
            {
                model.EncodeFormat = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.EncodeStringTransformDescription;
            }
        }
    }
}
