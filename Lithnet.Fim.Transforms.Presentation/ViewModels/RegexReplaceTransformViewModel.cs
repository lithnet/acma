using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Lithnet.Common.Presentation;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class RegexReplaceTransformViewModel : TransformViewModel
    {
        private RegexReplaceTransform model;

        public RegexReplaceTransformViewModel(RegexReplaceTransform model)
            : base(model)
        {
            this.model = model;
        }

        public string FindPattern
        {
            get
            {
                return model.FindPattern;
            }
            set
            {
                model.FindPattern = value;
            }
        }

        public string ReplacePattern
        {
            get
            {
                return model.ReplacePattern;
            }
            set
            {
                model.ReplacePattern = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.RegexReplaceTransformDescription;
            }
        }
    }
}
