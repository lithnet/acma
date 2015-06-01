using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class GetDNComponentTransformViewModel : TransformViewModel
    {
        private GetDNComponentTransform model;

        public GetDNComponentTransformViewModel(GetDNComponentTransform model)
            : base(model)
        {
            this.model = model;
        }

        public int ComponentIndex
        {
            get
            {
                return model.ComponentIndex;
            }
            set
            {
                model.ComponentIndex = value;
            }
        }

        public RdnFormat RdnFormat
        {
            get
            {
                return model.RdnFormat;
            }
            set
            {
                model.RdnFormat = value;
            }
        }

        public Direction Direction
        {
            get
            {
                return model.Direction;
            }
            set
            {
                model.Direction = value;
            }
        }
        
        public override string TransformDescription
        {
            get
            {
                return strings.GetDNComponentTransformDescription;
            }
        }
    }
}
