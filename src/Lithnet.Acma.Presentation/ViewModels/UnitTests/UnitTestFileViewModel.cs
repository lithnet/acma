using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;
using Microsoft.Win32;
using System.ComponentModel;
using System.Linq;
using Lithnet.Acma;
using Lithnet.Acma.Presentation;
using System.Collections.ObjectModel;
using System.Collections;
using Lithnet.Transforms;
using Lithnet.Transforms.Presentation;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;
using Lithnet.Acma.TestEngine;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestFileViewModel : ViewModelBase<UnitTestFile>
    {
        private UnitTestObjectsViewModel unitTestsViewModel;

        public UnitTestFileViewModel(UnitTestFile model)
            : base(model)
        {
            ActiveConfig.DB.CanCache = true;
            this.Initialize();
            this.UnitTestObjects = new UnitTestObjectsViewModel(this.Model.UnitTestObjects);
            ActiveConfig.DB.CanCache = false;
        }

        private void Initialize()
        {
            this.IgnorePropertyHasChanged.Add("UnitTests");
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.Commands.AddItem("AddUnitTest", t => this.UnitTestObjects.AddUnitTest());
            this.Commands.AddItem("AddUnitTestGroup", t => this.UnitTestObjects.AddUnitTestGroup());
            this.Commands.AddItem("Paste", t => this.UnitTestObjects.Paste(), t => this.UnitTestObjects.CanPaste());
            this.PasteableTypes.Add(typeof(UnitTest));
            this.PasteableTypes.Add(typeof(UnitTestGroup));
        }

        public string FileName
        {
            get
            {
                return this.Model.FileName;
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Unit Tests");
            }
        }

        public string ConfigVersion
        {
            get
            {
                return this.Model.ConfigVersion;
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

        public UnitTestObjectsViewModel UnitTestObjects
        {
            get
            {
                return this.unitTestsViewModel;
            }
            set
            {
                if (this.unitTestsViewModel != null)
                {
                    this.UnregisterChildViewModel(this.unitTestsViewModel);
                }

                this.unitTestsViewModel = value;
                this.RegisterChildViewModel(this.unitTestsViewModel);
            }
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.UnitTestObjects;
            }
        }

        private void New()
        {
            this.Model = new UnitTestFile();
        }

    }
}
