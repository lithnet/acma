using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class SidToDomainTransformViewModel : TransformViewModel
    {
        private SidToDomainTransform model;

        public SidToDomainTransformViewModel(SidToDomainTransform model)
            : base(model)
        {
            this.model = model;
        }

        public DomainFormat Format
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

        public override string TransformDescription
        {
            get
            {
                return strings.SidToDomainTransformDescription;
            }
        }
    }
}
