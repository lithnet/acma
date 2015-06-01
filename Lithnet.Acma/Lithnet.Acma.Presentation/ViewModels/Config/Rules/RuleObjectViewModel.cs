using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public abstract class RuleObjectViewModel : ViewModelBase<RuleObject>
    {
        bool canUseProposedValues;

        public RuleObjectViewModel(RuleObject model, bool canUseProposedValues)
            : base(model)
        {
            this.canUseProposedValues = canUseProposedValues;
        }

        public abstract string DisplayName { get; }

        public abstract string DisplayNameLong { get; }

        public bool CanUseProposedValues
        {
            get
            {
                if (this.Parent == null)
                {
                    return this.canUseProposedValues;
                }
                else
                {
                    RuleObjectViewModel ruleParent = this.Parent as RuleObjectViewModel;

                    if (ruleParent != null)
                    {
                        return ruleParent.CanUseProposedValues;
                    }
                    else
                    {
                        return this.canUseProposedValues;
                    }
                }
            }
        }

        protected override bool CanMoveDown()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override bool CanMoveUp()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
