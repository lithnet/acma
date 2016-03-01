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
using System.IO;

namespace Lithnet.Acma.Service
{
    public partial class ServiceMain : ServiceBase
    {
        private ServiceHost serviceHost = null;
        private FileSystemWatcher configFileWatcher = null;
        private bool configUpdateQueued = false;

        public ServiceMain()
        {
            InitializeComponent();
        }

        internal string ServicePath
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
        }


        internal string ConfigFile
        {
            get
            {
                string configFile = ConfigurationManager.AppSettings["configfile"];

                if (configFile == null)
                {
                    configFile = Path.Combine(this.ServicePath, "config.acmax");
                }

                return configFile;
            }
        }

        internal string LogFile
        {
            get
            {
                string logFile = ConfigurationManager.AppSettings["logFile"];

                if (logFile == null)
                {
                    logFile = Path.Combine(this.ServicePath, @"Logs\acma-service.log");
                }

                return logFile;
            }
        }

        internal string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["acmadb"].ConnectionString;
            }
        }

        protected override void OnStart(string[] args)
        {
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();

            try
            {
                Logger.LogPath = this.LogFile;
                Logger.LogLevel = LogLevel.Debug;
                Logger.WriteLine("Service starting");

                this.ConnectToDatabase();
                this.LoadConfiguration();


                this.serviceHost = new ServiceHost(typeof(AcmaWCF));

                this.serviceHost.Authorization.ServiceAuthorizationManager = new AuthorizationManager();
                this.serviceHost.Open();

                this.StartFileSystemWatcher();

                AcmaExternalExitEvent.StartEventQueue();
                Logger.WriteLine("Service started");
            }
            catch (Exception ex)
            {
                Logger.WriteLine("An error occurred while starting the service");
                Logger.WriteException(ex);
            }
        }

        private void LoadConfiguration()
        {
            if (!System.IO.File.Exists(this.ConfigFile))
            {
                throw new FileNotFoundException(string.Format("The configuration file '{0}' could not be found. Ensure the file is accessible by the service and the path specified in the app.config file is correct", this.ConfigFile));
            }

            ActiveConfig.LoadXml(this.ConfigFile);
            this.configUpdateQueued = false;
            Logger.WriteLine("Loaded configuration from {0}", this.ConfigFile);

        }

        private void ConnectToDatabase()
        {
            if (this.ConnectionString == null)
            {
                throw new InvalidOperationException("The connection string was not specified");
            }

            ActiveConfig.DB = new AcmaDatabase(this.ConnectionString);
            ActiveConfig.DB.CanCache = true;

            Logger.WriteLine("Connected to {0}", ActiveConfig.DB.ServerName);
        }

        private void StartFileSystemWatcher()
        {
            this.configFileWatcher = new FileSystemWatcher();
            this.configFileWatcher.Path = Path.GetDirectoryName(this.ConfigFile);
            this.configFileWatcher.Filter = Path.GetFileName(this.ConfigFile);
            this.configFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            this.configFileWatcher.Changed += configFileWatcher_Changed;
            this.configFileWatcher.EnableRaisingEvents = true;
        }

        private void configFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!this.configUpdateQueued)
            {
                this.configUpdateQueued = true;
                Logger.WriteLine("Detected config file change. Will reload when no export is in progress");
                AcmaWCF.ConfigLock.WaitOne();

                this.LoadConfiguration();
            }
            else
            {
                Logger.WriteLine("Detected config file change. Reload already queued");
            }
        }

        protected override void OnStop()
        {
            Logger.WriteLine("Service shutting down");

            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
                this.serviceHost = null;
            }

            if (this.configFileWatcher != null)
            {
                this.configFileWatcher.EnableRaisingEvents = false;
            }

            AcmaExternalExitEvent.CompleteEventQueue();
        }

        internal void Start(string[] args)
        {
            this.OnStart(args);
        }
    }
}
