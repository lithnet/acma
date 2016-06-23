using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.New, "AcmaDatabase")]
    public class NewAcmaDatabase : Cmdlet
    {
        public NewAcmaDatabase()
        {
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();
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

        protected override void ProcessRecord()
        {
            try
            {
                DBInstallUpgrader upgrader = new DBInstallUpgrader(this.ServerName);
                upgrader.CreateDB(this.DatabaseName);

                Console.WriteLine("Database created");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
