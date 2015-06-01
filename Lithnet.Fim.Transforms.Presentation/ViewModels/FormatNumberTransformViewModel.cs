
namespace Lithnet.Fim.Transforms.Presentation
{
    public class FormatNumberTransformViewModel : TransformViewModel  
    {
        private FormatNumberTransform model;

        public FormatNumberTransformViewModel(FormatNumberTransform model)
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


        public override string TransformDescription
        {
            get
            {
                return strings.FormatNumberTransformDescription;
            }
        }
    }
}
