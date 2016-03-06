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
using Lithnet.Acma.ServiceModel;
using System.Threading;
using Microsoft.Win32;

namespace Lithnet.Acma.Service
{
    public partial class ServiceMain : ServiceBase
    {
        private ServiceHost syncServiceHost = null;
        private ServiceHost acmaServiceHost = null;
        private FileSystemWatcher configFileWatcher = null;
        private bool configUpdateQueued = false;

        internal static object Lock = new object();

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
                if (ConfigurationManager.ConnectionStrings["acmadb"] != null)
                {
                    return ConfigurationManager.ConnectionStrings["acmadb"].ConnectionString;
                }
                else
                {
                    return null;
                }
            }
        }

        internal bool AutoConfigReload
        {
            get
            {
                if (ConfigurationManager.AppSettings["autoconfigreload"] == null)
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["autoconfigreload"]);
                }
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

                this.StartSyncServiceHost();
                this.StartAcmaServiceHost();

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

        private void StartSyncServiceHost()
        {
            this.syncServiceHost = new ServiceHost(typeof(AcmaSyncService));
            this.syncServiceHost.AddServiceEndpoint(typeof(IAcmaSyncService), SyncServiceConfig.NetNamedPipeBinding, SyncServiceConfig.NamedPipeUri);
            if (this.syncServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            {
                this.syncServiceHost.Description.Behaviors.Add(SyncServiceConfig.ServiceMetadataDisabledBehavior);
            }

            if (this.syncServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>() == null)
            {
                this.syncServiceHost.Description.Behaviors.Add(SyncServiceConfig.ServiceDebugBehavior);
            }

            this.syncServiceHost.Authorization.ServiceAuthorizationManager = new SyncServiceAuthorizationManager();
            this.syncServiceHost.Open();
        }

        private void StartAcmaServiceHost()
        {
            this.acmaServiceHost = new ServiceHost(typeof(AcmaService), new Uri(AcmaServiceConfig.NetTcpUri));
            this.acmaServiceHost.AddServiceEndpoint(typeof(IAcmaService), AcmaServiceConfig.NetTcpBinding, "");
            if (this.acmaServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            {
                this.acmaServiceHost.Description.Behaviors.Add(AcmaServiceConfig.ServiceMetadataDisabledBehavior);
            }

            if (this.acmaServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>() == null)
            {
                this.acmaServiceHost.Description.Behaviors.Add(AcmaServiceConfig.ServiceDebugBehavior);
            }

            this.acmaServiceHost.Authorization.ServiceAuthorizationManager = new AcmaServiceAuthorizationManager();
            this.acmaServiceHost.Open();
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
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Lithnet\Acma");

                if (key == null)
                {
                    throw new InvalidOperationException("The database settings were not found in the registry. Re-run the installer, or specify a connection string in the application config file");
                }

                string dbname = (string)key.GetValue("DatabaseName", null);
                string serverName = (string)key.GetValue("ServerName", null);

                if (dbname == null || serverName == null)
                {
                    throw new InvalidOperationException("The database settings were not found in the registry. Re-run the installer, or specify a connection string in the application config file");
                }

                ActiveConfig.DB = new AcmaDatabase(serverName, dbname);
            }
            else
            {
                ActiveConfig.DB = new AcmaDatabase(this.ConnectionString);
            }

            ActiveConfig.DB.CanCache = true;

            Logger.WriteLine("Connected to {0}", ActiveConfig.DB.ServerName);
        }

        private void StartFileSystemWatcher()
        {
            if (this.AutoConfigReload)
            {
                this.configFileWatcher = new FileSystemWatcher();
                this.configFileWatcher.Path = Path.GetDirectoryName(this.ConfigFile);
                this.configFileWatcher.Filter = Path.GetFileName(this.ConfigFile);
                this.configFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                this.configFileWatcher.Changed += configFileWatcher_Changed;
                this.configFileWatcher.EnableRaisingEvents = true;
            }
        }

        private void configFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!this.configUpdateQueued)
            {
                this.configUpdateQueued = true;
                Logger.WriteLine("Detected config file change. Queuing reload");

                try
                {
                    Monitor.Enter(ServiceMain.Lock);
                    Thread.Sleep(2000);
                    try
                    {
                        this.LoadConfiguration();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Automatic config file reload failed: {0}", ex.Message);
                    }
                }
                finally
                {
                    Monitor.Exit(ServiceMain.Lock);
                }
            }
            else
            {
                Logger.WriteLine("Detected config file change. Reload already queued");
            }
        }

        protected override void OnStop()
        {
            Logger.WriteLine("Service shutting down");

            if (this.syncServiceHost != null)
            {
                this.syncServiceHost.Close();
                this.syncServiceHost = null;
            }

            if (this.acmaServiceHost != null)
            {
                this.acmaServiceHost.Close();
                this.acmaServiceHost = null;
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
