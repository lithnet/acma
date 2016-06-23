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
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class ConstructorsViewModel : ListViewModel<ExecutableConstructorObjectViewModel, ExecutableConstructorObject>
    {
        private Constructors typedModel;

        public ConstructorsViewModel(Constructors model)
            : base()
        {
            this.typedModel = model;
            this.SetCollectionViewModel((IList)model, t => this.ViewModelResolver(t));

            this.Commands.AddItem("AddConstructorGroup", t => this.AddConstructorGroup());
            this.Commands.AddItem("AddDVConstructor", t => this.AddDVConstructor());
            this.Commands.AddItem("AddSIAConstructor", t => this.AddSIAConstructor());
            this.Commands.AddItem("AddVDConstructor", t => this.AddVDConstructor());
            this.Commands.AddItem("AddRLConstructor", t => this.AddRLConstructor());
            this.Commands.AddItem("AddUVConstructor", t => this.AddUVConstructor());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.PasteableTypes.Add(typeof(DeclarativeValueConstructor));
            this.PasteableTypes.Add(typeof(SequentialIntegerAllocationConstructor));
            this.PasteableTypes.Add(typeof(UniqueValueConstructor));
            this.PasteableTypes.Add(typeof(AttributeValueDeleteConstructor));
            this.PasteableTypes.Add(typeof(ReferenceLookupConstructor));
            this.PasteableTypes.Add(typeof(AttributeConstructorGroup));
            this.OnModelRemoved += ConstructorsViewModel_OnModelRemoved;
        }

        private void ConstructorsViewModel_OnModelRemoved(ListViewModelChangedEventArgs args)
        {
            ExecutableConstructorObject constructor = args.Model as ExecutableConstructorObject;

            if (constructor != null)
            {
                RemoveIDsFromCache(constructor);
            }
        }

        private void RemoveIDsFromCache(ExecutableConstructorObject constructor)
        {
            UniqueIDCache.RemoveItem(constructor, ExecutableConstructorObject.CacheGroupName);

            if (constructor is AttributeConstructorGroup)
            {
                foreach(ExecutableConstructorObject child in ((AttributeConstructorGroup)constructor).Constructors)
                {
                    this.RemoveIDsFromCache(child);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Constructors");
            }
        }

        public void AddDVConstructor()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
            }

            this.Add(new DeclarativeValueConstructor(), true);
        }

        public void AddSIAConstructor()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
            }
            
            this.Add(new SequentialIntegerAllocationConstructor(), true);
        }

        public void AddUVConstructor()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
            }
            
            this.Add(new UniqueValueConstructor(), true);
        }

        public void AddVDConstructor()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
            }
            
            this.Add(new AttributeValueDeleteConstructor(), true);
        }

        public void AddRLConstructor()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
            }
            
            this.Add(new ReferenceLookupConstructor(), true);
        }

        public void AddConstructorGroup()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
            }
            
            this.Add(new AttributeConstructorGroup(), true);
        }

        private ExecutableConstructorObjectViewModel ViewModelResolver(ExecutableConstructorObject model)
        {
            if (model is AttributeConstructorGroup)
            {
                return new AttributeConstructorGroupViewModel(model as AttributeConstructorGroup);
            } 
            else if (model is AttributeValueDeleteConstructor)
            {
                return new AttributeValueDeleteConstructorViewModel(model as AttributeValueDeleteConstructor);
            }
            else if (model is DeclarativeValueConstructor)
            {
                return new DeclarativeValueConstructorViewModel(model as DeclarativeValueConstructor);
            }
            else if (model is ReferenceLookupConstructor)
            {
                return new ReferenceLookupConstructorViewModel(model as ReferenceLookupConstructor);
            }
            else if (model is UniqueValueConstructor)
            {
                return new UniqueValueConstructorViewModel(model as UniqueValueConstructor);
            }
            else if (model is SequentialIntegerAllocationConstructor)
            {
                return new SequentialIntegerAllocationConstructorViewModel(model as SequentialIntegerAllocationConstructor);
            }
            else
            {
                throw new ArgumentException("Unknown object type", "model");
            }
        }
    }
}
