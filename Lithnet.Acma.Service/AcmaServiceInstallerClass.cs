using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace Lithnet.Acma.Service
{
    [RunInstaller(true)]
    public partial class AcmaServiceInstallerClass : System.Configuration.Install.Installer
    {
        public AcmaServiceInstallerClass()
        {
            InitializeComponent();
        }
    }
}
