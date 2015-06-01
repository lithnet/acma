using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class GroupStringsToADGroupTypeTransformViewModel : TransformViewModel
    {
        private GroupStringsToADGroupTypeTransform model;

        public GroupStringsToADGroupTypeTransformViewModel(GroupStringsToADGroupTypeTransform model)
            : base(model)
        {
            this.model = model;
        }


        public override string TransformDescription
        {
            get
            {
                return strings.GroupStringToADGroupTypeTransformDescription;
            }
        }
    }
}
