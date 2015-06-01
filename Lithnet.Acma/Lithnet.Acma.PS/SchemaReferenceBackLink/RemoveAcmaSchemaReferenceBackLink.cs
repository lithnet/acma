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
    [Cmdlet(VerbsCommon.Remove, "AcmaSchemaReferenceBackLink")]
    public class RemoveAcmaSchemaReferenceBackLink : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The object class containing the back-link source attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string SourceObjectClass { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The attribute containing the back-link source value",
            Position = 1,
            ValueFromPipeline = false)]
        public string SourceAttribute { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The target class containing the target attribute",
            Position = 2,
            ValueFromPipeline = false)]
        public string TargetObjectClass { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The target attribute to set the value on",
            Position = 3,
            ValueFromPipeline = false)]
        public string TargetAttribute { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaObjectClass sourceObjectClass = ActiveConfig.DB.GetObjectClass(this.SourceObjectClass);
                AcmaSchemaAttribute sourceAttribute = ActiveConfig.DB.GetAttribute(this.SourceAttribute);
                AcmaSchemaObjectClass targetObjectClass = ActiveConfig.DB.GetObjectClass(this.TargetObjectClass);
                AcmaSchemaAttribute targetAttribute = ActiveConfig.DB.GetAttribute(this.TargetAttribute);
                
                AcmaSchemaReferenceLink link = ActiveConfig.DB.GetReferenceLink(sourceObjectClass, sourceAttribute, targetObjectClass, targetAttribute);

                ActiveConfig.DB.DeleteReferenceLink(link);

                ActiveConfig.DB.ClearCache();
                Console.WriteLine("Reference link created");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
