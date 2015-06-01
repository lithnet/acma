using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma
{
    public class AcmaInheritedAttributeChange : AttributeChangeDetached
    {
        public AcmaInheritedAttributeChange(AcmaSchemaAttribute attribute, AttributeModificationType modificationType, IList<ValueChange> valueChanges)
            : base(attribute.Name, modificationType, valueChanges)
        {
        }
    }
}
