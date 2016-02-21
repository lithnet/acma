using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSequencesViewModel : ListViewModel<AcmaSequenceViewModel, AcmaSequence>
    {
        private IBindingList model;

        public AcmaSequencesViewModel(IBindingList model)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(this.model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddSequence", t =>this.AddSequence());
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Sequences");
            }
        }

        public void AddSequence()
        {
            NewSequenceWindow window = new NewSequenceWindow();
            NewAcmaSequenceViewModel vm = new NewAcmaSequenceViewModel(window);
            window.DataContext = vm;
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                   AcmaSequence sequence = ActiveConfig.DB.CreateSequence(vm.Name, vm.StartValue, vm.Increment, vm.MinValue, vm.MaxValue, vm.IsCycleEnabled);
                   this.Add(sequence);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not create the sequence\n\n" + ex.Message);
                }
            }
        }

        public void DeleteSequence(AcmaSequence model)
        {
            this.Remove(model);
            ActiveConfig.DB.Commit();
        }

        private AcmaSequenceViewModel ViewModelResolver(AcmaSequence model)
        {
            return new AcmaSequenceViewModel(model, this);
        }
    }
}