using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class RemoveDiacriticsTransformViewModel : TransformViewModel
    {
        private RemoveDiacriticsTransform model;

        public RemoveDiacriticsTransformViewModel(RemoveDiacriticsTransform model)
            : base(model)
        {
            this.model = model;
        }

        public override string TransformDescription
        {
            get
            {
                return strings.RemoveDiacriticsTransformDescription;
            }
        }
    }
}
