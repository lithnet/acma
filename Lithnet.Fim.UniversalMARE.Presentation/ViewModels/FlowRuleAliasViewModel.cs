using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Fim.UniversalMARE;
using System;

namespace Lithnet.Fim.UniversalMARE.Presentation
{
    public class FlowRuleAliasViewModel : ViewModelBase<FlowRuleAlias>
    {
        private FlowRuleAlias model;

        public FlowRuleAliasViewModel(FlowRuleAlias model)
            : base (model)
        {
            this.Commands.AddItem("DeleteAlias", t => this.DeleteAlias());
            this.model = model;
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.DisplayIcon = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Fim.UniversalMARE.Presentation;component/Resources/alias.png", UriKind.Absolute)); ;
        }

        public string DisplayName
        {
            get
            {
                return this.Alias;
            }
        }

        public string Alias
        {
            get
            {
                return model.Alias;
            }
            set
            {
                model.Alias = value;
            }
        }

        public string FlowRuleDefinition
        {
            get
            {
                return model.FlowRuleDefinition;
            }
            set
            {
                model.FlowRuleDefinition = value;
            }
        }

        private void DeleteAlias()
        {
            this.ParentCollection.Remove(this.model);
        }
    }
}
