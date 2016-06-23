using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class AcmaDatabaseViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowVM;

        private AcmaSchemaObjectsViewModel objectClasses;

        private AcmaSequencesViewModel sequences;

        private AcmaSchemaAttributesViewModel attributes;

        public AcmaDatabaseViewModel(MainWindowViewModel vm)
            : base()
        {
            this.mainWindowVM = vm;
            this.BeginIgnoreChanges();
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.ObjectClasses = new AcmaSchemaObjectsViewModel(ActiveConfig.DB.ObjectClassesBindingList);
            this.Attributes = new AcmaSchemaAttributesViewModel(ActiveConfig.DB.AttributesBindingList);
            this.Sequences = new AcmaSequencesViewModel(ActiveConfig.DB.SequencesBindingList);
            ActiveConfig.DB.CanCache = false;

            this.ResetChildViewModels();
            
            this.Commands.AddItem("ConnectToDatabase", t => vm.ConnectToDatabase(false));
        }

        public string DisplayName
        {
            get
            {
                return string.Format(string.Format("Database ({0})", ActiveConfig.DB.ServerName));
            }
        }

        public AcmaSchemaAttributesViewModel Attributes
        {
            get
            {
                return this.attributes;
            }
            set
            {
                if (this.attributes != null)
                {
                    this.UnregisterChildViewModel(this.attributes);
                }

                this.attributes = value;

                this.RegisterChildViewModel(this.attributes);
            }
        }

        public AcmaSchemaObjectsViewModel ObjectClasses
        {
            get
            { 
                return this.objectClasses; }
            set 
            {
                if (this.objectClasses != null)
                {
                    this.UnregisterChildViewModel(this.objectClasses);
                }

                this.objectClasses = value;

                this.RegisterChildViewModel(this.objectClasses);
            }
        }

        public AcmaSequencesViewModel Sequences
        {
            get
            {
                return this.sequences;
            }
            set
            {
                if (this.sequences != null)
                {
                    this.UnregisterChildViewModel(this.sequences);
                }

                this.sequences = value;

                this.RegisterChildViewModel(this.sequences);
            }
        }

        public AcmaConstantsViewModel Constants { get; set; }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.Attributes;
                yield return this.ObjectClasses;
                yield return this.Sequences;
                yield return this.Constants;
            }
        }
        
        private void ResetChildViewModels()
        {
            

            if (this.Constants != null)
            {
                this.Constants.SetCollectionViewModel(ActiveConfig.DB.ConstantsBindingList);
            }
            else
            {
                this.Constants = new AcmaConstantsViewModel(ActiveConfig.DB.ConstantsBindingList);
            }
        }
    }
}