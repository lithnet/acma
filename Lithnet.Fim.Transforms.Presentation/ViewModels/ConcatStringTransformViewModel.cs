using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class ConcatStringTransformViewModel : TransformViewModel
    {
        private ConcatStringTransform model;

        public ConcatStringTransformViewModel(ConcatStringTransform model)
            : base(model)
        {
            this.model = model;
        }

        public string Delimiter
        {
            get
            {
                return model.Delimiter;
            }
            set
            {
                model.Delimiter = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.ConcatStringTransformDescription;
            }
        }
    }
}
