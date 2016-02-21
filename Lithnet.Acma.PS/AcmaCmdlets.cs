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

namespace Lithnet.Acma.PS
{
    [RunInstaller(true)]
    public class AcmaCmdlets : PSSnapIn
    {
        public AcmaCmdlets()
            : base()
        {
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();
        }
        
        public override string Name
        {
            get
            {
                return "AcmaCmdlets";
            }
        }

        public override string Vendor
        {
            get
            {
                return "Lithnet";
            }
        }

        public override string Description
        {
            get
            {
                return "Lithnet ACMA PowerShell Cmdlets";
            }
        }

        protected override void OnAfterRollback(IDictionary savedState)
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                this.Uninstall64Extension();
            }

            base.OnAfterRollback(savedState);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                this.Uninstall64Extension();
            }

            base.OnAfterUninstall(savedState);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            // These overrides perform the installation of the 64-bit powershell registration when called from a 32-bit process running on a 64-bit system. 
            // The custom action of the MSI installer always calls the 32-bit installutil library, causing the 64-bit component to remain unregistered

            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                this.Install64Extension();
            }

            base.OnAfterInstall(savedState);
        }

        private void Install64Extension()
        {
            string path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string installUtilPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory().Replace("\\Framework\\", "\\Framework64\\");

            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo();
            si.Arguments = string.Format("\"{0}\"", path);
            si.FileName = System.IO.Path.Combine(installUtilPath, "installutil.exe");
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            si.UseShellExecute = false;
            p.StartInfo = si;
            p.Start();
            p.WaitForExit();
            string ouptput = p.StandardOutput.ReadToEnd();

            System.Diagnostics.Debug.WriteLine(ouptput);
        }

        private void Uninstall64Extension()
        {
            string path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string installUtilPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory().Replace("\\Framework\\", "\\Framework64\\");

            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo();
            si.Arguments = string.Format("/u \"{0}\"", path);
            si.FileName = System.IO.Path.Combine(installUtilPath, "installutil.exe");
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            si.UseShellExecute = false;
            p.StartInfo = si;
            p.Start();
            p.WaitForExit();
            string ouptput = p.StandardOutput.ReadToEnd();

            System.Diagnostics.Debug.WriteLine(ouptput);
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
