using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Lithnet.Acma;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using Lithnet.Fim;

namespace Lithnet.Acma.Service
{
    public partial class ServiceMain : ServiceBase
    {
        ServiceHost serviceHost = null;

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MmsAssemblyResolver.RegisterResolver();

            if (ActiveConfig.DB == null)
            {
                ActiveConfig.DB = new AcmaDatabase(ConfigurationManager.ConnectionStrings["acmadb"].ConnectionString);
            }

            if (ActiveConfig.XmlConfig == null)
            {
                ActiveConfig.LoadXml(ConfigurationManager.AppSettings["configfile"]);
            }

            this.serviceHost = new ServiceHost(typeof(AcmaWCF));
            this.serviceHost.Authorization.ServiceAuthorizationManager = new ServiceAuthorizationManager();
            this.serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
                this.serviceHost = null;
            }
        }

        internal void Start(string[] args)
        {
            this.OnStart(args);
        }
    }
}
