using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Fim.Core;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class NumberCalculationTransformViewModel : TransformViewModel
    {
        private NumberCalculationTransform model;

        public NumberCalculationTransformViewModel(NumberCalculationTransform model)
            : base(model)
        {
            this.model = model;
        }

        public NumberOperator Operator
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

        public long Value
        {
            get
            {
                return model.Value;
            }
            set
            {
                model.Value = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.NumberCalculationTransformDescription;
            }
        }
    }
}
