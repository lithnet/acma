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
    [Cmdlet(VerbsCommon.Set, "AcmaSchemaObjectClass")]
    public class SetAcmaSchemaObjectClass : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the object class",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter(
           Mandatory = true,
           HelpMessage = "Indicates whether the object can be undeleted",
           Position = 1,
           ValueFromPipeline = false)]
        public bool IsUndeletable { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(this.Name);
                objectClass.AllowResurrection = this.IsUndeletable;
                ActiveConfig.DB.Commit();
                ActiveConfig.DB.ClearCache();
                Console.WriteLine("Object class modified");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
