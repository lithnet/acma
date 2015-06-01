using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using System.IO;
using Microsoft.MetadirectoryServices;

namespace ExtensibleExitEventSample
{
    public class ObjectLoggerEvent : IAcmaExternalEventExtensible
    {
        public void Execute(RaisedEvent raisedEvent)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), string.Format("exit-event-{0}.log", raisedEvent.EventID));

            using (FileStream s = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter writer = new StreamWriter(s))
                {
                    if (raisedEvent.Source.AttributeChanges.Count > 0)
                    {
                        writer.WriteLine("Object modification: {0} - {1}", raisedEvent.Source.AcmaModificationType, raisedEvent.Source.DisplayText);

                        foreach (AttributeChange attributeChange in raisedEvent.Source.AttributeChanges)
                        {
                            writer.WriteLine("Attribute change: {0}: {1}", attributeChange.Name, attributeChange.ModificationType);

                            foreach (ValueChange valueChange in attributeChange.ValueChanges)
                            {
                                writer.WriteLine("\t{0}: {1}", valueChange.ModificationType, valueChange.Value); 
                            }
                        }

                        writer.WriteLine("--------------------------------------------------------");
                    }
                }
            }
        }

        public void OnEventRaised(RaisedEvent raisedEvent, MAObjectHologram sender)
        {
        }
    }
}
