using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using System.Collections;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsCommon.Get, "AcmaObject", DefaultParameterSetName = "GetResource")]
    public class GetAcmaObjectCmdLet : AcmaCmdletConnected
    {
        [Parameter(ParameterSetName = "GetResource", ValueFromPipeline = true, Mandatory = true, Position = 1)]
        public object ID { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = true, ValueFromPipeline = true, Position = 1)]
        [Parameter(ParameterSetName = "GetResourceByKeys", Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public string ObjectType { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = true, Position = 2)]
        public string AttributeName { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = true, Position = 3)]
        public object AttributeValue { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKeys", Mandatory = true, Position = 2)]
        public Hashtable AttributeValuePairs { get; set; }

        protected override void ProcessRecord()
        {
            if (!this.IsConnectionStatusOk(false))
            {
                return;
            }

            if (this.ID != null)
            {
                Guid? guidID;
                string stringID = this.ID as string;
                if (stringID != null)
                {
                    guidID = new Guid(stringID);
                }
                else
                {
                    guidID = this.ID as Guid?;

                    if (guidID == null)
                    {
                        throw new ArgumentException("The ID must be a GUID object or a GUID in string format");
                    }
                }

                MAObjectHologram maobject = ActiveConfig.DB.GetMAObjectOrDefault(new Guid(stringID));

                if (maobject == null)
                {
                    return;
                }

                this.WriteObject(new AcmaPSObject(maobject));
                return;
            }
            else if (this.AttributeValuePairs != null)
            {
                DBQueryGroup group = new DBQueryGroup(GroupOperator.All);

                foreach (object key in this.AttributeValuePairs.Keys)
                {
                    DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute((string)key), ValueOperator.Equals, this.AttributeValuePairs[key]);
                    group.AddChildQueryObjects(query);
                }

                IList<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count == 0)
                {
                    throw new NotFoundException();
                }
                else if (results.Count > 1)
                {
                    throw new InvalidOperationException("More than one object matched the given criteria. Use Get-AcmaObjects for returning multiple results");
                }
                else
                {
                    this.WriteObject(new AcmaPSObject(results.First()));
                    return;
                }
            }
            else
            {
                DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute(this.AttributeName), ValueOperator.Equals, this.AttributeValue);

                IList<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(query).ToList();

                if (results.Count == 0)
                {
                    throw new NotFoundException();
                }
                else if (results.Count > 1)
                {
                    throw new InvalidOperationException("More than one object matched the given criteria. Use Get-AcmaObjects for returning multiple results");
                }
                else
                {
                    this.WriteObject(new AcmaPSObject(results.First()));
                    return;
                }
            }
        }
    }
}