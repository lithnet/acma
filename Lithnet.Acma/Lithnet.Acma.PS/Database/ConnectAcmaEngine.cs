using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Logging;
using Lithnet.Fim;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommunications.Connect, "AcmaEngine")]
    public class ConnectAcmaEngine: Cmdlet 
    {
        public ConnectAcmaEngine()
        {
            MmsAssemblyResolver.RegisterResolver();
            this.LogLevel = Logging.LogLevel.Info;
        }

        [Parameter(
            Mandatory=true,
            HelpMessage="The name of the database to connect to",
            Position=0)]
        public string DatabaseName { get; set; }

        [Parameter(
            Mandatory=true,
            HelpMessage="The name of database server",
            Position=1)]
        public string ServerName { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The name of configuration file",
            Position = 2)]
        public string ConfigFile { get; set; }

        [Parameter(
           Mandatory = false,
           HelpMessage = "The name of log file",
           Position = 3)]
        public string LogFile { get; set; }


        [Parameter(
           Mandatory = false,
           HelpMessage = "The logging detail level",
           Position = 4)]
        public LogLevel LogLevel { get; set; }
        
        protected override void ProcessRecord()
        {
            try
            {
                Logger.CloseLog();

                Logger.LogPath = this.LogFile;
                Logger.LogLevel = this.LogLevel;
                Logger.WriteLine("Connecting to {0} on {1}", this.DatabaseName, this.ServerName);
                ActiveConfig.OpenDatabase(this.ServerName, this.DatabaseName);

                if (this.ConfigFile != null)
                {
                    Logger.WriteLine("Opening {0}", this.ConfigFile);
                    ActiveConfig.LoadXml(this.ConfigFile);
                }
                else
                {
                    ActiveConfig.XmlConfig = new XmlConfigFile();
                }

                Global.Connected = true;
                Global.DataContext = ActiveConfig.DB.MADataConext;
                ActiveConfig.DB.CanCache = true;
                WriteCommandDetail("Connected to server");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.OpenError, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
