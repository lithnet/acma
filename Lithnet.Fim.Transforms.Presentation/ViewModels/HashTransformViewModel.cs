using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class HashTransformViewModel : TransformViewModel
    {
        private HashTransform model;

        public HashTransformViewModel(HashTransform model)
            : base(model)
        {
            this.model = model;
        }

        public HashType HashType
        {
            get
            {
                return model.HashType;
            }
            set
            {
                model.HashType = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.HashTransformDescription;
            }
        }
    }
}
