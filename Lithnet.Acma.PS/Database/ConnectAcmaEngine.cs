using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Logging;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommunications.Connect, "AcmaEngine")]
    public class ConnectAcmaEngine: AcmaCmdletConnected 
    {
        public ConnectAcmaEngine()
        {
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

                this.Connect(this.DatabaseName, this.ServerName, this.ConfigFile, true);
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.OpenError, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
