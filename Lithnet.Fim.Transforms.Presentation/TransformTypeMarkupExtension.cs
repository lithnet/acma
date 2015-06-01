using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Reflection;
using Lithnet.Fim.Transforms;
using Lithnet.Common.Presentation;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class TransformTypeMarkupExtension : MarkupExtension
    {
        private readonly Type type;

        public TransformTypeMarkupExtension(Type type)
        {
            this.type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            TypeMarkupExtension typeExtension = new TypeMarkupExtension(typeof(Transform));
            IEnumerable<TypeDescriptionWrapper> result = typeExtension.ProvideValue(null) as IEnumerable<TypeDescriptionWrapper>;

            if (result == null)
            {
                throw new ArgumentException("result");
            }

            if (TransformGlobal.HostProcessSupportsLoopbackTransforms)
            {
                return result;
            }
            else
            {
                return result.Where(t => !(t.Value.GetCustomAttributes(true).Any(u => u.GetType() == typeof(LoopbackTransformAttribute))));
            }
        }
    }
}
