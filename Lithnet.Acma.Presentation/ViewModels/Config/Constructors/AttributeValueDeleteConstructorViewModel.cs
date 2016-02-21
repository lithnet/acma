using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.Presentation
{
    public class AttributeValueDeleteConstructorViewModel : AttributeConstructorViewModel
    {
        public AttributeValueDeleteConstructorViewModel(AttributeValueDeleteConstructor model)
            : base(model)
        {
        }
    }
}
