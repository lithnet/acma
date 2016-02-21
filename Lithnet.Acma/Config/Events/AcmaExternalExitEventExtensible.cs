using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Lithnet.Acma;
using System.Diagnostics;
using Microsoft.MetadirectoryServices;
using Lithnet.Logging;
using System.Reflection;

namespace Lithnet.Acma
{
    [DataContract(Name = "exit-event-external-extensible", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Exit event (extensible)")]
    public class AcmaExternalExitEventExtensible : AcmaExternalExitEvent
    {
        public AcmaExternalExitEventExtensible()
        {
            this.Initialize();
        }

        [DataMember(Name = "extension-path")]
        public string ExtensionPath { get; set; }

        [DataMember(Name = "class-name")]
        public string ClassName { get; set; }

        private IAcmaExternalEventExtensible externalEvent;

        private IAcmaExternalEventExtensible ExternalEvent
        {
            get
            {
                if (this.externalEvent == null)
                {
                    Assembly assembly = Assembly.LoadFrom(this.ExtensionPath);

                    foreach (Type t in assembly.GetExportedTypes().Where(u => typeof(IAcmaExternalEventExtensible).IsAssignableFrom(u)))
                    {
                        if (t.Name == this.ClassName)
                        {
                            this.externalEvent = (IAcmaExternalEventExtensible)Activator.CreateInstance(t);
                            return this.externalEvent;
                        }
                    }

                    throw new EntryPointNotFoundException(string.Format(
                        "The assembly {0} did not contain a class named {1} that inherits from IAcmaExternalEventExtensible",
                        assembly.FullName,
                        this.ClassName));
                }

                return this.externalEvent;
            }
        }

        protected override void ExecuteInternal(RaisedEvent raisedEvent)
        {
            this.ExternalEvent.Execute(raisedEvent);
        }

        internal override void OnEventRaised(RaisedEvent raisedEvent, MAObjectHologram sender)
        {
            this.ExternalEvent.OnEventRaised(raisedEvent, sender);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}
