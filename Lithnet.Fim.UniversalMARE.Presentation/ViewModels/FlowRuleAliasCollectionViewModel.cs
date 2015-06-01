using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Fim.UniversalMARE;
using System.Collections;

namespace Lithnet.Fim.UniversalMARE.Presentation
{
    public class FlowRuleAliasCollectionViewModel : ListViewModel<FlowRuleAliasViewModel, FlowRuleAlias>
    {
        private FlowRuleAliasKeyedCollection model;

        public FlowRuleAliasCollectionViewModel(FlowRuleAliasKeyedCollection model)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel((IList)this.model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddAlias", t => this.AddAlias());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.DisplayIcon = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Fim.UniversalMARE.Presentation;component/Resources/alias.png", UriKind.Absolute)); ;
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Flow Rule Aliases");
            }
        }

        public void AddAlias()
        {
            NewAliasWindow window = new NewAliasWindow();
            NewAliasViewModel vm = new NewAliasViewModel(this, window);
            window.DataContext = vm;
            window.ShowDialog();
        }

        private FlowRuleAliasViewModel ViewModelResolver(FlowRuleAlias model)
        {
            return new FlowRuleAliasViewModel(model);
        }
    }
}