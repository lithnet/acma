using System.Collections.Generic;
using System.ComponentModel;
using Lithnet.Common.ObjectModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class MVBooleanToBitmaskTransformViewModel : TransformViewModel
    {
        private MVBooleanToBitmaskTransform model;

        public MVBooleanToBitmaskTransformViewModel(MVBooleanToBitmaskTransform model)
            : base(model)
        {
            this.model = model;

            if (this.model.Flags == null)
            {
                this.model.Flags = new List<FlagValue>();
            }

            this.Flags = new BindingList<FlagValue>(this.model.Flags);
        }

        public BindingList<FlagValue> Flags { get; private set; }

        public long DefaultValue
        {
            get
            {
                return model.DefaultValue;
            }
            set
            {
                model.DefaultValue = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.MVBooleanToBitmaskTransformDescription;
            }
        }
    }
}
