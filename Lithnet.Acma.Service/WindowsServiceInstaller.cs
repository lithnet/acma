using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Lithnet.Acma.Service.Installer
{
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller : System.Configuration.Install.Installer
    {
        public WindowsServiceInstaller()
        {
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();

            InitializeComponent();

            ServiceProcessInstaller process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.User;
            ServiceInstaller service = new ServiceInstaller();
            service.ServiceName = "acma";
            service.DisplayName = "ACMA Service";
            service.StartType = ServiceStartMode.Automatic;

            Installers.Add(process);
            Installers.Add(service);
        }
    }
}