using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Configuration.Install;
using System.IO;

namespace Lithnet.Acma.PS
{
    [RunInstaller(true)]
    public class PSModuleInstaller : Installer
    {
        public PSModuleInstaller()
            : base()
        {
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            string psdPath = Environment.ExpandEnvironmentVariables(@"%systemroot%\sysnative\WindowsPowerShell\v1.0\Modules\AcmaPS\AcmaPS.psd1");
            string text = File.ReadAllText(psdPath);
            string installdir = this.Context.Parameters["INSTALLDIR"];

            if (installdir.EndsWith(@"\\"))
            {
                installdir = installdir.Substring(0, installdir.Length - 1);
            }

            text = text.Replace("!EDITORPATH!", installdir);
            File.WriteAllText(psdPath, text);
            
            base.OnAfterInstall(savedState);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            this.RegisterEventSource();
        }

        private void RegisterEventSource()
        {
            if (!EventLog.SourceExists("ACMA"))
            {
                EventLog.CreateEventSource("ACMA", "Application");
            }
        }
    }
}
