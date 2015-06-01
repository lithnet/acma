using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;

namespace Lithnet.Fim
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
            MmsAssemblyResolver.RegisterResolver();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            this.RegisterEventSource();
        }

        private void RegisterEventSource()
        {
            if (!EventLog.SourceExists("Lithnet.Fim.MmsResovler"))
            {
                EventLog.CreateEventSource("Lithnet.Fim.MmsResovler", "Application");
            }
        }
    }
}
