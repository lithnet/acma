using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace Lithnet.Fim.Core.AssemblyResolver
{
    public class MmsAssemblyResolver : Installer
    {
        static MmsAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Equals("Microsoft.MetadirectoryServicesEx", StringComparison.InvariantCultureIgnoreCase))
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\FIMSynchronizationService\Parameters", false);

                if (key != null)
                {
                    string path = key.GetValue("Path", null) as string;

                    if (path != null)
                    {
                        Assembly mmsAssemby = Assembly.LoadFrom(Path.Combine(path, "bin\\assemblies\\Microsoft.MetadirectoryServicesEx.dll"));
                        if (mmsAssemby != null)
                        {
                            return mmsAssemby;
                        }
                    }

                }

                throw new FileNotFoundException(@"The Microsoft.MetadirectoryServicesEx.dll file could not be found on this system. Ensure the FIM synchronization service has been installed, or the DLL registered in the GAC");
            }

            return null;
        }
    }
}
