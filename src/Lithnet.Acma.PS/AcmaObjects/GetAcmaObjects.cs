using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Get, "AcmaObjects")]
    public class GetAcmaObjectsCmdLet : Cmdlet 
    {
        [Parameter(Mandatory=true,
            HelpMessage="The IDs of the AcmaObject",
            HelpMessageBaseName="IDs",
            Position=0,
            ParameterSetName="ObjectsByID",
            ValueFromPipeline=true)]
        public Guid[] IDs { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The query to search for the objects",
            HelpMessageBaseName = "Query",
            Position = 0,
            ParameterSetName = "ObjectsByDBQuery",
            ValueFromPipeline = true)]
        public DBQueryObject DBQuery { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                if (this.IDs != null)
                {
                    foreach (Guid id in this.IDs)
                    {
                        MAObjectHologram maobject = ActiveConfig.DB.GetMAObjectOrDefault(id);

                        if (maobject == null)
                        {
                            ErrorRecord error = new ErrorRecord(new NotFoundException(), "ObjectNotFound", ErrorCategory.ObjectNotFound, id);
                            ThrowTerminatingError(error);
                        }

                        WriteObject(new AcmaPSObject(maobject));
                    }
                }
                else if (this.DBQuery != null)
                {
                    IEnumerable<MAObjectHologram> objects = ActiveConfig.DB.GetMAObjectsFromDBQuery(this.DBQuery);
                    foreach (MAObjectHologram item in objects)
                    {
                        WriteObject(new AcmaPSObject(item));
                    }
                }
                else
                {
                    this.DBQuery = new DBQueryByValue(ActiveConfig.DB.GetAttribute("deleted"), ValueOperator.Equals, 0);
                    IEnumerable<MAObjectHologram> objects = ActiveConfig.DB.GetMAObjectsFromDBQuery(this.DBQuery);
                    foreach (MAObjectHologram item in objects)
                    {
                        WriteObject(new AcmaPSObject(item));
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, this.IDs);
                ThrowTerminatingError(error);
            }
        }
    }
}
