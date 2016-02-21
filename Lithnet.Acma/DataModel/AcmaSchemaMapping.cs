using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lithnet.Acma.DataModel
{
    public partial class AcmaSchemaMapping
    {
        private IBindingList safetyRulesBindingList;

        public IBindingList SafetyRuleBindingList
        {
            get
            {
                if (this.safetyRulesBindingList == null)
                {
                    this.safetyRulesBindingList = this.SafetyRules.GetNewBindingList();
                }
                return this.safetyRulesBindingList;
            }
        }

        public bool HasSafetyRules
        {
            get
            {
                return this.SafetyRules.Any();
            }
        }

        public bool IsInherited
        {
            get
            {
                return this.InheritedAttribute != null;
            }
        }

        public override string ToString()
        {
            if (this.IsInherited)
            {
                // person:ouName->sapOrganizationalUnit->organizationalUnit:displayName
                return string.Format("{0}:{1}->{2}->{3}:{4}", this.ObjectClass.Name, this.Attribute.Name, this.InheritanceSourceAttribute.Name, this.InheritanceSourceObjectClass.Name, this.InheritedAttribute.Name);
            }
            else
            {
                return string.Format("{0}:{1}", this.ObjectClass.Name, this.Attribute.Name);
            }
        }
    }
}
