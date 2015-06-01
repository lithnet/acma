using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Fim.Transforms;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class FormatStringTransformViewModel : TransformViewModel
    {
        private FormatStringTransform model;

        public FormatStringTransformViewModel(FormatStringTransform model)
            : base(model)
        {
            this.model = model;
        }

        public string Format
        {
            get
            {
                return model.Format;
            }
            set
            {
                model.Format = value;
            }
        }

        public MutivalueBehaviour UserDefinedMultivalueInputBehaviour
        {
            get
            {
                return this.model.UserDefinedMultivalueInputBehaviour;
            }
            set
            {
                this.model.UserDefinedMultivalueInputBehaviour = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.FormatStringTransformDescription;
            }
        }
    }
}
