using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class TransformViewModel : ViewModelBase<Transform>
    {
        private Transform model;

        public TransformViewModel(Transform model)
            : base(model)
        {
            this.Commands.AddItem("DeleteTransform", t => this.DeleteTransform());
            this.model = model;
            this.EnableCutCopy();
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.TransformTest = new TransformTestViewModel(model);
            this.DisplayIcon = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Fim.Transforms.Presentation;component/Resources/transform.png", UriKind.Absolute)); ;
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", this.Id, this.model.GetType().GetTypeDescription());
            }
        }

        public string Type
        {
            get
            {

                return this.model.GetType().GetTypeDescription();
            }
        }

        protected override bool CanMoveDown()
        {
            return true;
        }

        protected override bool CanMoveUp()
        {
            return true;
        }

        public string MVBehaviour
        {
            get
            {

                if (!this.model.SupportsMultivaluedInput)
                {
                    return "This transform does not support multivalued input attributes";
                }
                else if (this.model.HandlesOwnMultivaluedProcessing)
                {
                    return "This transform process multivalued attribute values together in a single pass";
                }
                else
                {
                    return "This transform passes over each value in an multivalued attribute individually";
                }
            }
        }

        public bool IsLoopbackSupported
        {
            get
            {
                return this.model.ImplementsLoopbackProcessing;
            }
        }

        public string SupportedInputTypes
        {
            get
            {
                return this.model.AllowedInputTypes.Where(t => t != ExtendedAttributeType.Undefined).Select(t => t.GetEnumDescription()).ToCommaSeparatedString();
            }
        }

        public string PossibleReturnTypes
        {
            get
            {
                return this.model.PossibleReturnTypes.Where(t => t != ExtendedAttributeType.Undefined).Select(t => t.GetEnumDescription()).ToCommaSeparatedString();
            }
        }

        public string Id
        {
            get
            {
                return model.ID;
            }
            set
            {
                model.ID = value;
            }
        }

        public virtual string TransformDescription
        {
            get
            {
                return string.Empty;
            }
        }

        public TransformTestViewModel TransformTest { get; set; }

        private void DeleteTransform()
        {
            this.ParentCollection.Remove(this.model);
        }
    }
}
