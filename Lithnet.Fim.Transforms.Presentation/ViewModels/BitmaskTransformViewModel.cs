
namespace Lithnet.Fim.Transforms.Presentation
{
    public class BitmaskTransformViewModel : TransformViewModel  
    {
        private BitmaskTransform model;

        public BitmaskTransformViewModel(BitmaskTransform model)
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

        public BitwiseOperation Operation
        {
            get
            {
                return model.Operation;
            }
            set
            {
                model.Operation = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.BitmaskTransformDescription;
            }
        }
    }
}
