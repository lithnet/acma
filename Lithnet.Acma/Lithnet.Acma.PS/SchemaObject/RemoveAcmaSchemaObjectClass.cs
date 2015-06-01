using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Fim.Core;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Remove, "AcmaSchemaObjectClass")]
    public class RemoveAcmaSchemaObject : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the object class",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                if (this.ShouldProcess("Are you sure you want to delete this object class? This will delete all objects of this class"))
                {
                    if (this.Force || this.ShouldContinue("Are you sure you want to delete this object class? This will delete all objects of this class", "Confirm object class delete"))
                    {
                        AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(this.Name);
                        ActiveConfig.DB.DeleteObjectClass(objectClass);
                        ActiveConfig.DB.Commit();
                        ActiveConfig.DB.ClearCache();
                        Console.WriteLine("Object class deleted");
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
