using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.TestEngine;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class UnitTestObjectsViewModel : ListViewModel<UnitTestObjectViewModel, UnitTestObject>
    {
        public UnitTestObjectsViewModel(IList<UnitTestObject> model)
            : base()
        {
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddUnitTest", t => this.AddUnitTest());
            this.PasteableTypes.Add(typeof(UnitTest));
            this.PasteableTypes.Add(typeof(UnitTestGroup));
            this.OnModelRemoved += ViewModel_OnModelRemoved;
        }

        private void ViewModel_OnModelRemoved(ListViewModelChangedEventArgs args)
        {
            UnitTestObject item = args.Model as UnitTestObject;

            if (item != null)
            {
                this.RemoveIDsFromCache(item);
            }
        }

        private void RemoveIDsFromCache(UnitTestObject item)
        {
            UniqueIDCache.RemoveItem(item, UnitTestObject.CacheGroupName);

            if (item is UnitTestGroup)
            {
                foreach (UnitTestObject child in ((UnitTestGroup)item).UnitTestObjects)
                {
                    this.RemoveIDsFromCache(child);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Unit Tests");
            }
        }
        
        public string Path
        {
            get
            {
                UnitTestGroupViewModel parent = this.Parent as UnitTestGroupViewModel;

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

        public string ParentPath
        {
            get
            {
                UnitTestGroupViewModel parent = this.Parent as UnitTestGroupViewModel;

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

        private UnitTestObjectViewModel ViewModelResolver(UnitTestObject model)
        {
            if (model is UnitTest)
            {
                return new UnitTestViewModel((UnitTest)model);
            }
            else if (model is UnitTestGroup)
            {
                return new UnitTestGroupViewModel((UnitTestGroup)model);
            }
            else
            {
                throw new InvalidOperationException("Unknown unit test object type");
            }
        }

        public void AddUnitTest()
        {
            UnitTest item = new UnitTest();
            item.ID = "New unit test";
            this.Parent.IsExpanded = true;
            this.IsExpanded = true;
            this.Add(item, true);
        }

        public void AddUnitTestGroup()
        {
            UnitTestGroup item = new UnitTestGroup();
            item.ID = "New test group";
            this.Parent.IsExpanded = true; 
            this.IsExpanded = true;
            this.Add(item, true);
        }
    }
}
