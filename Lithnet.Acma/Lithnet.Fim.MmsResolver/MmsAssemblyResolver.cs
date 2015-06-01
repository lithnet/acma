using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace Lithnet.Fim
{
    public static class MmsAssemblyResolver
    {
        public static void RegisterResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            EventLog eventLog = new EventLog("Application");
            eventLog.Source = "Lithnet.Fim.MmsResovler";

            if (args.Name.StartsWith("Microsoft.MetadirectoryServicesEx", StringComparison.InvariantCultureIgnoreCase))
            {
#if DEBUG
                eventLog.WriteEntry(string.Format("Lithnet.Fim.MMSAssemblyResolver searching for: {0}", args.Name), EventLogEntryType.Information, 1);
#endif
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\FIMSynchronizationService\Parameters", false);

                if (key != null)
                {
                    string path = key.GetValue("Path", null) as string;

                    if (path != null)
                    {
                        Assembly mmsAssemby = Assembly.LoadFrom(Path.Combine(path, "bin\\assemblies\\Microsoft.MetadirectoryServicesEx.dll"));
                        if (mmsAssemby != null)
                        {
#if DEBUG
                            eventLog.WriteEntry(string.Format("Lithnet.Fim.MMSAssemblyResolver found: {0}", mmsAssemby.FullName), EventLogEntryType.Information, 2);
#endif
                            return mmsAssemby;
                        }
                    }
                }
                else
                {
                    // FIM sync installed on this box. Check some other locations
                    // ProgramFiles\Lithnet\MMS.dll
                    // 
                }

#if DEBUG
                eventLog.WriteEntry(string.Format("Lithnet.Fim.MMSAssemblyResolver could not find Microsoft.MetadirectoryServicesEx.dll"), EventLogEntryType.Error, 3);
#endif

                throw new FileNotFoundException(@"The Microsoft.MetadirectoryServicesEx.dll file could not be found on this system. Ensure the FIM synchronization service has been installed, or the DLL registered in the GAC");
            }

            return null;
        }
    }
}
