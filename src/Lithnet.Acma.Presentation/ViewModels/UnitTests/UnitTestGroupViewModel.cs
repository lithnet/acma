using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Lithnet.Acma.DataModel;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.Acma.TestEngine;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestGroupViewModel : UnitTestObjectViewModel
    {
        private UnitTestObjectsViewModel unitTestObjects;

        private UnitTestGroup typedModel;

        public UnitTestGroupViewModel(UnitTestGroup model)
            : base(model)
        {
            this.typedModel = model;
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.UnitTestObjects = new UnitTestObjectsViewModel(this.typedModel.UnitTestObjects);
            this.Commands.AddItem("AddUnitTest", t => this.UnitTestObjects.AddUnitTest());
            this.Commands.AddItem("AddUnitTestGroup", t => this.UnitTestObjects.AddUnitTestGroup());
            this.Commands.AddItem("Paste", t => this.UnitTestObjects.Paste(), t => this.UnitTestObjects.CanPaste());
            this.Commands.AddItem("Delete", t => this.Delete());
            this.EnableCutCopy();
        }

       
        public UnitTestObjectsViewModel UnitTestObjects
        {
            get
            {
                return this.unitTestObjects;
            }
            set
            {
                if (this.unitTestObjects != null)
                {
                    this.UnregisterChildViewModel(this.unitTestObjects);
                }

                this.unitTestObjects = value;

                this.RegisterChildViewModel(this.unitTestObjects);
            }
        }

        private void Delete()
        {
            if (this.ParentCollection != null)
            {
                this.ParentCollection.Remove(this.Model);
            }
        }
    }
}
