using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;

namespace Lithnet.Acma.Installers
{
    [RunInstaller(true)]
    public partial class AcmaInstallerActions : System.Configuration.Install.Installer
    {
        public AcmaInstallerActions()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
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
