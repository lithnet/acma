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
    [Cmdlet(VerbsCommon.Add, "AcmaSchemaAttribute")]
    public class AddAcmaSchemaAttribute : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The data type",
            Position = 1,
            ValueFromPipeline = false)]
        public AcmaPSAttributeType Type { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Indicates if the attribute is multivalued",
            Position = 2,
            ValueFromPipeline = false)]
        public bool IsMultivalued{ get; set; }

        [Parameter(
           Mandatory = false,
           HelpMessage = "Indicates if the attribute is indexed",
           Position = 3,
           ValueFromPipeline = false)]
        public bool IsIndexed { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The operations supported on the attribute",
            Position = 4,
            ValueFromPipeline = false)]
        public AcmaAttributeOperation? Operation { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                bool indexable = false;
                ExtendedAttributeType attributeType;

                switch (this.Type)
                {
                    case AcmaPSAttributeType.String:
                        attributeType = ExtendedAttributeType.String;
                        indexable = true;
                        break;

                    case AcmaPSAttributeType.StringNotIndexable:
                        attributeType = ExtendedAttributeType.String;
                        indexable = false;
                        break;

                    case AcmaPSAttributeType.Integer:
                        attributeType = ExtendedAttributeType.Integer;
                        indexable = true;
                        break;

                    case AcmaPSAttributeType.Boolean:
                        attributeType = ExtendedAttributeType.Boolean;
                        indexable = false;
                        break;

                    case AcmaPSAttributeType.Reference:
                        attributeType = ExtendedAttributeType.Reference;
                        indexable = true;
                        break;

                    case AcmaPSAttributeType.Binary:
                        attributeType = ExtendedAttributeType.Binary;
                        indexable = true;
                        break;

                    case AcmaPSAttributeType.BinaryNotIndexable:
                        attributeType = ExtendedAttributeType.Binary;
                        indexable = false;
                        break;

                    case AcmaPSAttributeType.DateTime:
                        attributeType = ExtendedAttributeType.DateTime;
                        indexable = true;
                        break;

                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }

                AcmaAttributeOperation operation;
                if (this.Operation.HasValue)
                {
                    operation = this.Operation.Value;
                }
                else
                {
                    operation = AcmaAttributeOperation.ImportExport;
                }

                ActiveConfig.DB.CreateAttribute(this.Name, attributeType, this.IsMultivalued, operation, indexable, this.IsIndexed);
                ActiveConfig.DB.ClearCache();
                Console.WriteLine("Attribute added");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
