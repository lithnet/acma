using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.Presentation
{
    public abstract class AcmaEventViewModel : ViewModelBase<AcmaEvent>
    {
        public AcmaEventViewModel(AcmaEvent model)
            : base(model)
        {
            this.Commands.AddItem("DeleteEvent", t => this.Delete());

            this.EnableCutCopy();
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Id))
                {
                    return string.Format("Undefined event");
                }
                else
                {
                    return this.Id;
                }
            }
        }

        public bool IsDisabled
        {
            get
            {
                return this.Model.IsDisabled;
            }

            set
            {
                this.Model.IsDisabled = value;
                this.RaisePropertyChanged("DisplayIcon");
            }
        }

        public string Id
        {
            get
            {
                return this.Model.ID;
            }

            set
            {
                this.Model.ID = value;
            }
        }

        protected override bool CanMoveDown()
        {
            return true;
        }

        protected override bool CanMoveUp()
        {
            return true;
        }

        private void Delete()
        {
            this.ParentCollection.Remove(this.Model);
        }

    }
}
