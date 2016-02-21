using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public abstract class RuleViewModel : RuleObjectViewModel
    {
        private Rule typedModel;

        public RuleViewModel(Rule model, bool canUseProposedValues)
            : base(model, canUseProposedValues)
        {
            this.Commands.AddItem("DeleteRule", t => this.DeleteRule());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.typedModel = model;
            this.EnableCutCopy();
        }

        public string Type
        {
            get
            {
                return this.Model.GetType().GetTypeDescription();
            }
        }

        private void DeleteRule()
        {
            this.ParentCollection.Remove(this.Model);
        }
    }
}
