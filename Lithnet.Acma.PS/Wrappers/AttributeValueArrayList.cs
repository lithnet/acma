using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Management.Automation;

namespace Lithnet.Acma.PS
{
    public class AttributeValueArrayList : ArrayList
    {
        public AttributeValueArrayList()
            : base()
        {
        }

        public AttributeValueArrayList(ICollection c)
            : base(c)
        {
        }

        public override void Remove(object obj)
        {
            if (base.Contains(obj))
            {
                // obj is already a unique identifier
                base.Remove(obj);
                return;
            }

            AcmaPSObject rmaObject = obj as AcmaPSObject;
            if (rmaObject != null)
            {
                // obj is an existing object
                base.Remove(rmaObject);
                return;
            }
        }
    }
}
