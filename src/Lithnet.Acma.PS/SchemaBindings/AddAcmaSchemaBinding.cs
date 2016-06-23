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
    [Cmdlet(VerbsCommon.Add, "AcmaSchemaBinding")]
    public class AddAcmaSchemaBinding : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ParameterSetName = "InheritedBinding",
            ValueFromPipeline = false)]
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the attribute",
            Position = 0,
            ParameterSetName = "SimpleBinding",
            ValueFromPipeline = false)]
        public string Attribute { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The object class to bind the attribute to",
            Position = 1,
            ParameterSetName = "InheritedBinding",
            ValueFromPipeline = false)]
        [Parameter(
            Mandatory = true,
            HelpMessage = "The object class to bind the attribute to",
            Position = 1,
            ParameterSetName = "SimpleBinding",
            ValueFromPipeline = false)]
        public string ObjectClass { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The object class to inherit the attribute from",
            Position = 2,
            ParameterSetName = "InheritedBinding",
            ValueFromPipeline = false)]
        public string InheritanceSourceClass { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The attribute that will contain the referenced object to inherit from",
            Position = 3,
            ParameterSetName = "InheritedBinding",
            ValueFromPipeline = false)]
        public string InheritanceSourceReference { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The attribute to inherit",
            Position = 4,
            ParameterSetName = "InheritedBinding",
            ValueFromPipeline = false)]
        public string InheritanceSourceAttribute { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(this.ObjectClass);
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(this.Attribute);
                
                if (this.InheritanceSourceAttribute == null)
                {
                    ActiveConfig.DB.CreateMapping(objectClass, attribute, objectClass.MappingsBindingList);
                }
                else
                {
                    AcmaSchemaAttribute inheritedAttribute = ActiveConfig.DB.GetAttribute(this.InheritanceSourceAttribute);
                    AcmaSchemaObjectClass inheritanceSourceObjectClass = ActiveConfig.DB.GetObjectClass(this.InheritanceSourceClass);
                    AcmaSchemaAttribute innheritanceReference = ActiveConfig.DB.GetAttribute(this.InheritanceSourceReference);

                    ActiveConfig.DB.CreateMapping(objectClass, attribute, innheritanceReference, inheritanceSourceObjectClass, inheritedAttribute, objectClass.MappingsBindingList);
                }

                ActiveConfig.DB.ClearCache();
                Console.WriteLine("Binding added");
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
