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
    [Cmdlet(VerbsCommon.New, "AcmaQuery")]
    public class NewAcmaQueryCmdLet : Cmdlet 
    {
        [Parameter(Mandatory=true,
            HelpMessage="The attribute name to query",
            HelpMessageBaseName="Attribute",
            Position=0,
            ValueFromPipeline=false)]
        public string AttributeName { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The operator to use for the query",
            HelpMessageBaseName = "value",
            Position = 1,
            ValueFromPipeline = false)]
        public ValueOperator Operator { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The value to query",
            HelpMessageBaseName = "value",
            Position = 2,
            ValueFromPipeline = false)]
        public string Value { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(this.AttributeName);
                DBQueryByValue query = new DBQueryByValue(attribute, this.Operator, new ValueDeclaration(this.Value));
                
                WriteObject(query);
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(error);
            }
        }
    }
}
