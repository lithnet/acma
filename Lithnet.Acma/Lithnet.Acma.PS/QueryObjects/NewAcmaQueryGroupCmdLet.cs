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
    [Cmdlet(VerbsCommon.New, "AcmaQueryGroup")]
    public class NewAcmaQueryGroupCmdLet : Cmdlet 
    {
        [Parameter(Mandatory = true,
            HelpMessage = "The operator to use for the query group",
            HelpMessageBaseName = "operator",
            Position = 0,
            ValueFromPipeline = false)]
        public GroupOperator Operator { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The queries to add to the group",
            HelpMessageBaseName = "queries",
            Position = 1,
            ValueFromPipeline = false)]
        public DBQueryObject[] QueryObjects { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                DBQueryGroup group = new DBQueryGroup(this.Operator);
                
                foreach(DBQueryObject item in this.QueryObjects)
                {
                    group.DBQueries.Add(item);
                }
                
                WriteObject(group);
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(error);
            }
        }
    }
}
