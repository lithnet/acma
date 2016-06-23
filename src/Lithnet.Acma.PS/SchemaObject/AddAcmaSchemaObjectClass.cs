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
    [Cmdlet(VerbsCommon.Add, "AcmaSchemaObjectClass")]
    public class AddAcmaSchemaObjectClass : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the object class",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter(
           Mandatory = false,
           HelpMessage = "Indicates whether the object can be undeleted",
           Position = 1,
           ValueFromPipeline = false)]
        public bool IsUndeletable { get; set; }


        [Parameter(
           Mandatory = false,
           HelpMessage = "The name of the object class to shadow from",
           Position = 2,
           ValueFromPipeline = false)]
        public string ShadowFrom { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaObjectClass shadowFromClass = null;

                if (this.ShadowFrom != null)
                {
                    shadowFromClass = ActiveConfig.DB.GetObjectClass(this.ShadowFrom);
                }

                ActiveConfig.DB.CreateObjectClass(this.Name, this.IsUndeletable, shadowFromClass);
                ActiveConfig.DB.ClearCache();
                Console.WriteLine("Object class added");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
