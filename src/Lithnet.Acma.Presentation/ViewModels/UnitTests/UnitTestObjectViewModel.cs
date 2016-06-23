using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using Lithnet.Acma.TestEngine;

namespace Lithnet.Acma.Presentation
{
    public abstract class UnitTestObjectViewModel : ViewModelBase<UnitTestObject>
    {
        public UnitTestObjectViewModel(UnitTestObject model)
            : base(model)
        {
        }

        public string TestId
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

        public string Path
        {
            get
            {
                UnitTestObjectsViewModel parent = this.ParentCollection as UnitTestObjectsViewModel;

                if (parent == null)
                {
                    return this.DisplayName;
                }
                else
                {
                    return parent.Path + "\\" + this.DisplayName;
                }
            }
        }

        public string ParentPath
        {
            get
            {
                UnitTestObjectsViewModel parent = this.ParentCollection as UnitTestObjectsViewModel;

                if (parent == null)
                {
                    return string.Empty;
                }
                else
                {
                    return parent.Path;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0}", this.TestId);
            }
        }
       
        public string Description
        {
            get
            {
                return this.Model.Description;
            }
            set
            {
                this.Model.Description = value;
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
