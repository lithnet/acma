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
    [Cmdlet(VerbsCommon.Set, "AcmaSchemaAttribute")]
    public class SetAcmaSchemaAttribute : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter(
           Mandatory = false,
           HelpMessage = "The new operation type of the attribute",
           Position = 1,
           ValueFromPipeline = false)]
        public AcmaAttributeOperation? Operation { get; set; }

        [Parameter(
           Mandatory = false,
           HelpMessage = "The index status of the attribute",
           Position = 2,
           ValueFromPipeline = false)]
        public bool? IsIndexed { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(this.Name);

                if (this.Operation.HasValue)
                {
                    attribute.Operation = this.Operation.Value;
                }

                if (this.IsIndexed.HasValue)
                {
                    if (attribute.IsInAVPTable || attribute.IsBuiltIn || !attribute.IsIndexable)
                    {
                        throw new InvalidOperationException("Cannot modify the indexing parameter on the specified attribute");
                    }

                    if (this.IsIndexed.Value)
                    {
                        ActiveConfig.DB.CreateIndex(attribute);
                    }
                    else
                    {
                        ActiveConfig.DB.DeleteIndex(attribute);
                    }

                    ActiveConfig.DB.RefreshEntity(attribute);
                }

                ActiveConfig.DB.Commit();
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
