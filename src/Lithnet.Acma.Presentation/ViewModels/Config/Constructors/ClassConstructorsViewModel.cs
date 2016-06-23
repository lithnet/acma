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

namespace Lithnet.Acma.Presentation
{
    public class ClassConstructorsViewModel : ListViewModel<ClassConstructorViewModel, ClassConstructor>
    {
        private ClassConstructors typedModel;

        public ClassConstructorsViewModel(ClassConstructors model)
            : base()
        {
            this.SetCollectionViewModel((IList)model, t => this.ViewModelResolver(t));
            this.typedModel = model;
            this.Commands.AddItem("AddConstructor", t => this.AddConstructor(), t => this.CanAddConstructor());
            this.PasteableTypes.Add(typeof(ClassConstructor));
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Class Constructors");
            }
        }

        public bool CanAddConstructor()
        {
            return ActiveConfig.DB.ObjectClasses.Except(ActiveConfig.XmlConfig.ClassConstructors.Select(t => t.ObjectClass)).Any();
        }

        public void AddConstructor()
        {
            NewClassConstructorWindow window = new NewClassConstructorWindow();
            NewClassConstructorViewModel vm = new NewClassConstructorViewModel(window);
            window.DataContext = vm;
            bool? result = window.ShowDialog();

            if (result != null && result.Value)
            {
                if (vm.ObjectClass != null)
                {
                    ClassConstructor constructor = new ClassConstructor();
                    constructor.ObjectClass = vm.ObjectClass;
                    this.Add(constructor);
                }
            }
        }

        private ClassConstructorViewModel ViewModelResolver(ClassConstructor model)
        {
            return new ClassConstructorViewModel(model);
        }

        public override bool CanPaste()
        {
            if (base.CanPaste())
            {
                string className = ClipboardManager.GetObjectIdentifierFromClipboard();

                if(ActiveConfig.DB.ObjectClasses.Any(t => t.Name == className))
                {
                    if (this.Any(t => t.Model.ObjectClass.Name == className))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }                
            }
            else
            {
                return false;
            }
        }
    }
}