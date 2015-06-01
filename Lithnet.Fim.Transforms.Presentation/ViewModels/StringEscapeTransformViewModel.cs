using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class StringEscapeTransformViewModel : TransformViewModel
    {
        private StringEscapeTransform model;

        public StringEscapeTransformViewModel(StringEscapeTransform model)
            : base(model)
        {
            this.model = model;
        }

        public StringEscapeType EscapeType
        {
            get
            {
                return model.EscapeType;
            }
            set
            {
                model.EscapeType = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.StringEscapeTransformDescription;
            }
        }
    }
}
