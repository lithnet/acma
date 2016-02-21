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

namespace Lithnet.Acma
{
    [DataContract(Name = "exit-event-external-cmd", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Exit event (external command)")]
    public class AcmaExternalExitEventCmd : AcmaExternalExitEvent
    {
        public AcmaExternalExitEventCmd()
        {
            this.Initialize();
        }

        [DataMember(Name = "command-line")]
        public string CommandLine { get; set; }

        [DataMember(Name = "arguments")]
        public ValueDeclaration Arguments { get; set; }

        protected override void ExecuteInternal(RaisedEvent raisedEvent)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = this.CommandLine;
            startInfo.Arguments = raisedEvent.RaisedEventProperties.ContainsKey("arguments") ? raisedEvent.RaisedEventProperties["arguments"] as string : null;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Logger.WriteLine("Starting process {0} {1}", this.CommandLine, startInfo.Arguments);
            Process p = new Process();
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();
            Logger.WriteLine("Process returned exit code {0}", LogLevel.Info, p.ExitCode);

            if (p.ExitCode != 0)
            {
                throw new Win32Exception(p.ExitCode);
            }
        }

        internal override void OnEventRaised(RaisedEvent raisedEvent, MAObjectHologram sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (raisedEvent == null)
            {
                throw new ArgumentNullException("raisedEvent");
            }

            IList<object> arguments = this.Arguments.Expand(sender);
            
            if (arguments.Count > 1)
            {
                throw new ArgumentException(
                    string.Format("The command argument declaration returned too many values. Only a single value is allowed to be returned from the declaration\nDeclaration:{0}\nObject:{1}",
                    this.Arguments.Declaration,
                    sender.DisplayText));
            }

            string argument = arguments.FirstOrDefault() as string;

            if (argument != null)
            {
                raisedEvent.RaisedEventProperties.Add("arguments", argument);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.Arguments = new ValueDeclaration();
        }
    }
}
