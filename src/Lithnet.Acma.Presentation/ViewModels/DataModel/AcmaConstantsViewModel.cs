using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;

namespace Lithnet.Acma.Presentation
{
    public class AcmaConstantsViewModel : ViewModelBase
    {
        private IBindingList model;

        public AcmaConstantsViewModel(IBindingList model)
            : base()
        {
            this.model = model;

            this.Commands.AddItem("Save", t => this.Save());

            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public void SetCollectionViewModel(IBindingList bindingList)
        {
            this.model = bindingList;
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Constants");
            }
        }

        public IBindingList Model
        {
            get
            {
                return this.model;
            }
        }

        public void Save()
        {
            ActiveConfig.DB.Commit();
        }
    }
}