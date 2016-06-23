using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Host;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsData.Update, "AcmaDatabase", SupportsShouldProcess = true)]
    public class UpdateAcmaDatabase : Cmdlet
    {
        public UpdateAcmaDatabase()
        {
        }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the database",
            Position = 0,
            ValueFromPipeline = false)]
        public string DatabaseName { get; set; }

        [Parameter(
           Mandatory = true,
           HelpMessage = "The name of the SQL server",
           Position = 1,
           ValueFromPipeline = false)]
        public string ServerName { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Global.Connected)
                {
                    throw new InvalidOperationException("Cannot update a database while a connection is open. Call the Disconnect-AcmaEngine cmdlet.");
                }

                DBInstallUpgrader upgrader = new DBInstallUpgrader(this.ServerName);
                Version currentdbVersion = upgrader.GetDBVersion(this.DatabaseName);

                if (currentdbVersion == DBInstallUpgrader.DBVersion)
                {
                    Console.WriteLine("The database is already at the current version: " + currentdbVersion.ToString());
                }
                else
                {
                    Console.WriteLine("Performing database upgrade from version " + currentdbVersion.ToString());

                    if (this.ShouldProcess(string.Format("The operation will perform an upgrade of the database to version {0}", DBInstallUpgrader.DBVersion), "Ensure the database has been backed up before proceeding", "Backup database"))
                    {
                        if (this.Force || this.ShouldContinue("Continue with the database upgrade? Please ensure the database has been backed up", "Confirm database update"))
                        {
                            upgrader.UpgradeDB(this.DatabaseName);
                            currentdbVersion = upgrader.GetDBVersion(this.DatabaseName);
                            Console.WriteLine("Database upgraded to version " + currentdbVersion.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Upgrade aborted");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
