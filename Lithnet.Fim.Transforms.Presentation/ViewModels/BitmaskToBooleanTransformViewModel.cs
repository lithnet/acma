
namespace Lithnet.Fim.Transforms.Presentation
{
    public class BitmaskToBooleanTransformViewModel : TransformViewModel  
    {
        private BitmaskToBooleanTransform model;

        public BitmaskToBooleanTransformViewModel(BitmaskToBooleanTransform model)
            : base(model)
        {
            this.model = model;
        }

        public long Flag
        {
            get
            {
                return model.Flag;
            }
            set
            {
                model.Flag = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.BitmaskToBooleanTransformDescription;
            }
        }
    }
}
