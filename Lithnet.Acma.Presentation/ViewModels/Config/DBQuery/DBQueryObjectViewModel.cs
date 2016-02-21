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
    public class DBQueryObjectViewModel : ViewModelBase<DBQueryObject>
    {
        public DBQueryObjectViewModel(DBQueryObject model)
            : base(model)
        {
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
