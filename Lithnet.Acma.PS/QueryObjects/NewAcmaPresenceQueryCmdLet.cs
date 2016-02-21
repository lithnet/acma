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
    [Cmdlet(VerbsCommon.New, "AcmaPresenceQuery")]
    public class NewAcmaPresenceQueryCmdLet : Cmdlet 
    {
        public NewAcmaPresenceQueryCmdLet()
        {
            this.valueOperator = ValueOperator.IsPresent;
        }

        private ValueOperator valueOperator;
        
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
        public ValueOperator Operator
        {
            get
            {
                return this.valueOperator;
            }
            set
            {
                if (value != ValueOperator.IsPresent && value != ValueOperator.NotPresent)
                {
                    throw new ArgumentException("An IsPresent or NotPresent operator must be specified");
                }
                else
                {
                    this.valueOperator = value;
                }
            }
        }
       
        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(this.AttributeName);
                DBQueryByValue query = new DBQueryByValue(attribute, this.Operator, new ValueDeclaration());
                
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
