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
using Lithnet.MetadirectoryServices;
using Lithnet.Logging;

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
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();

            if (ActiveConfig.DB == null)
            {
                ActiveConfig.DB = new AcmaDatabase(ConfigurationManager.ConnectionStrings["acmadb"].ConnectionString);
            }

            ActiveConfig.LoadXml(ConfigurationManager.AppSettings["configfile"]);

            Logger.LogPath = ConfigurationManager.AppSettings["logFile"];
            Logger.LogLevel = LogLevel.Debug;

            this.serviceHost = new ServiceHost(typeof(AcmaWCF));
            this.LoadSerializerSurrogate();

            this.serviceHost.Authorization.ServiceAuthorizationManager = new ServiceAuthorizationManager();
            this.serviceHost.Open();
        }

        private void LoadSerializerSurrogate()
        {

            foreach (var endpoint in this.serviceHost.Description.Endpoints)
            {
                foreach (var operation in endpoint.Contract.Operations)
                {
                    operation.Behaviors.Find<DataContractSerializerOperationBehavior>().DataContractSurrogate = new MmsSerializationSurrogate();
                }
            }
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
