using System;
using System.Windows;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace Lithnet.Fim.UniversalMARE.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
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

                MessageBox.Show(@"The Microsoft.MetadirectoryServicesEx.dll file could not be found on this system. Please copy the file from the bin\Assemblies folder from the FIM Synchronization Server, and place it in the ACMA Editor application directory");
                Environment.Exit(9000);
            }

            return null;
        }
    }
}