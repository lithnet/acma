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
            //this.LoadSerializerSurrogate();

            this.serviceHost.Authorization.ServiceAuthorizationManager = new ServiceAuthorizationManager();
            this.serviceHost.Open();
        }

        //private void LoadSerializerSurrogate()
        //{
        //    foreach (ServiceEndpoint ep in this.serviceHost.Description.Endpoints)
        //    {
        //        foreach (OperationDescription op in ep.Contract.Operations)
        //        {
        //            DataContractSerializerOperationBehavior dataContractBehavior =
        //                op.Behaviors.Find<DataContractSerializerOperationBehavior>()
        //                as DataContractSerializerOperationBehavior;
        //            if (dataContractBehavior != null)

        //            {
        //                dataContractBehavior.DataContractSurrogate = new MmsSerializationSurrogate();
        //            }
        //            else
        //            {
        //                dataContractBehavior = new DataContractSerializerOperationBehavior(op);
        //                dataContractBehavior.DataContractSurrogate = new MmsSerializationSurrogate();
        //                op.Behaviors.Add(dataContractBehavior);
        //            }
        //        }
        //    }
        //}

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
