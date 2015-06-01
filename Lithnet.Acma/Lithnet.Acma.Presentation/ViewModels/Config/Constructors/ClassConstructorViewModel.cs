using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class ClassConstructorViewModel : ViewModelBase<ClassConstructor>
    {
        private ConstructorsViewModel constructors;

        private DBQueryGroupViewModel resurrectionParameters;

        private AcmaExitEventsViewModel exitEvents;

        public ClassConstructorViewModel(ClassConstructor model)
            : base(model)
        {
            this.Constructors = new ConstructorsViewModel(this.Model.Constructors);
            this.ResurrectionParameters = new DBQueryGroupViewModel(this.Model.ResurrectionParameters, "Undelete parameters");
            this.ExitEvents = new AcmaExitEventsViewModel(this.Model.ExitEvents);
            
            this.Model.ObjectClass.PropertyChanged += ObjectClass_PropertyChanged;
            this.Commands.AddItem("DeleteConstructor", t => this.DeleteConstructor());
            this.EnableCutCopy();
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        private void ObjectClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged("ChildNodes");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0}", this.Name);
            }
        }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public bool Disabled
        {
            get
            {
                return this.Model.Disabled;
            }
            set
            {
                this.Model.Disabled = value;
                this.RaisePropertyChanged("DisplayIcon");
            }
        }

        public IEnumerable ChildNodes
        {
            get
            {
                if (this.Model.ObjectClass.AllowResurrection)
                {
                    yield return this.ResurrectionParameters;
                }
                
                yield return this.Constructors;
                yield return this.ExitEvents;
            }
        }

        public DBQueryGroupViewModel ResurrectionParameters
        {
            get
            {
                return this.resurrectionParameters;
            }
            set
            {
                if (this.resurrectionParameters != null)
                {
                    this.UnregisterChildViewModel(this.resurrectionParameters);
                }

                this.resurrectionParameters = value;

                this.RegisterChildViewModel(this.resurrectionParameters);
            }
        }

        public ConstructorsViewModel Constructors
        {
            get
            {
                return this.constructors;
            }
            set
            {
                if (this.constructors != null)
                {
                    this.UnregisterChildViewModel(this.constructors);
                }

                this.constructors = value;
                this.RegisterChildViewModel(this.constructors);
            }
        }

        public AcmaExitEventsViewModel ExitEvents
        {
            get
            {
                return this.exitEvents;
            }
            set
            {
                if (this.exitEvents != null)
                {
                    this.UnregisterChildViewModel(this.exitEvents);
                }

                this.exitEvents = value;
                this.RegisterChildViewModel(this.exitEvents);
            }
        }

        private void DeleteConstructor()
        {
            try
            {
                if (MessageBox.Show("Are you are you want to delete this class constructor?\n\nAll objects beneath this object will be deleted. This operation cannot be undone", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    this.ParentCollection.Remove(this.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the object class\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
