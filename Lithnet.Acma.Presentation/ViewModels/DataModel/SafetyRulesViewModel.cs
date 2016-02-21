using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class SafetyRulesViewModel : ListViewModel<SafetyRuleViewModel, SafetyRule>
    {
        private AcmaSchemaMapping mapping;

        private IBindingList model;

        public SafetyRulesViewModel(IBindingList model, AcmaSchemaMapping mapping)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("Add", t => this.ShowCreateWindow());

            this.mapping = mapping;
        }

        public AcmaSchemaMapping Mapping
        {
            get
            {
                return this.mapping;
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Safety rules");
            }
        }

        public void ShowCreateWindow()
        {
            NewSafetyRuleWindow window = new NewSafetyRuleWindow();
            NewSafetyRuleViewModel vm = new NewSafetyRuleViewModel(window, this);
            window.DataContext = vm;
            window.ShowDialog();
        }

        public void AddSafetyRule(string name, string pattern, bool nullAllowed)
        {
           SafetyRule rule = ActiveConfig.DB.CreateSafetyRule(this.Mapping, name, pattern, nullAllowed, this.model);
           this.Add(rule);
        }

        private SafetyRuleViewModel ViewModelResolver(SafetyRule model)
        {
            return new SafetyRuleViewModel(model);
        }
    }
}