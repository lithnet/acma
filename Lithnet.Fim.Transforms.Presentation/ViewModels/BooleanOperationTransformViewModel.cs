using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class BooleanOperationTransformViewModel : TransformViewModel
    {
        private BooleanOperationTransform model;

        public BooleanOperationTransformViewModel(BooleanOperationTransform model)
            : base(model)
        {
            this.model = model;
        }

        public BitwiseOperation Operator
        {
            get
            {
                return model.Operator;
            }
            set
            {
                model.Operator = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.BooleanOperationTransformDescription;
            }
        }
    }
}
