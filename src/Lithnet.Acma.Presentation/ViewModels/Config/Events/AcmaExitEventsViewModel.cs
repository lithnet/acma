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
    public class AcmaExitEventsViewModel : ListViewModel<AcmaEventViewModel, AcmaEvent>
    {
        public AcmaExitEventsViewModel(IList<AcmaEvent> model)
            : base()
        {
            this.Commands.AddItem("Paste", t => this.Paste(), t => this.CanPaste());
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.Commands.AddItem("AddInternalEvent", t => this.AddInternalExitEvent());
            this.Commands.AddItem("AddExternalEventCmd", t => this.AddExternalExitEventCmd());
            this.Commands.AddItem("AddExternalEventExtensible", t => this.AddExternalExitEventExtenisble());
            this.PasteableTypes.Add(typeof(AcmaInternalExitEvent));
            this.PasteableTypes.Add(typeof(AcmaExternalExitEventCmd));
            this.PasteableTypes.Add(typeof(AcmaExternalExitEventExtensible));
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Exit events");
            }
        }

        public void AddInternalExitEvent()
        {
            this.Add(new AcmaInternalExitEvent(), true);
        }
       
        public void AddExternalExitEventCmd()
        {
            this.Add(new AcmaExternalExitEventCmd(), true);
        }

        public void AddExternalExitEventExtenisble()
        {
            this.Add(new AcmaExternalExitEventExtensible(), true);
        }

        private AcmaEventViewModel ViewModelResolver(AcmaEvent model)
        {
            if (model is AcmaInternalExitEvent)
            {
                return new AcmaInternalExitEventViewModel(model as AcmaInternalExitEvent);
            }
            else if (model is AcmaOperationEvent)
            {
                throw new InvalidOperationException("This collection cannot host AcmaOperationEvent models");
            }
            else if (model is AcmaExternalExitEventCmd)
            {
                return new AcmaExternalExitEventCmdViewModel(model as AcmaExternalExitEventCmd);
            }
            else if (model is AcmaExternalExitEventExtensible)
            {
                return new AcmaExternalExitEventExtensibleViewModel(model as AcmaExternalExitEventExtensible);
            }
            else
            {
                throw new InvalidOperationException("The specified model does not have a corresponding ViewModel");
            }
        }
    }
}
