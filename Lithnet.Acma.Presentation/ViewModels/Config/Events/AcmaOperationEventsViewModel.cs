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
    public class AcmaOperationEventsViewModel : ListViewModel<AcmaEventViewModel, AcmaEvent>
    {
        public AcmaOperationEventsViewModel(IList<AcmaEvent> model)
            : base()
        {
            this.Commands.AddItem("Paste", t => this.Paste(), t => this.CanPaste());
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.Commands.AddItem("AddOperationEvent", t => this.AddOperationEvent());
            this.PasteableTypes.Add(typeof(AcmaOperationEvent));
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Pre-operation events");
            }
        }

        public void AddOperationEvent()
        {
            this.Add(new AcmaOperationEvent(), true);
        }

        private AcmaEventViewModel ViewModelResolver(AcmaEvent model)
        {
            if (!(model is AcmaOperationEvent))
            {
                throw new InvalidOperationException("This collection can only contain OperationEvent models");
            }

            return new AcmaOperationEventViewModel(model as AcmaOperationEvent);
        }
    }
}
