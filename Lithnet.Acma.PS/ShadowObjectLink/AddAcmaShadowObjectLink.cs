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
    [Cmdlet(VerbsCommon.Add, "AcmaShadowObjectLink")]
    public class AddAcmaShadowObjectLink : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the shadow link",
            Position = 0,
            ValueFromPipeline = false)]
        public string Name { get; set; }

        [Parameter(
          Mandatory = true,
          HelpMessage = "The name of the shadow object class",
          Position = 1,
          ValueFromPipeline = false)]
        public string ShadowObjectClass { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the reference attribute",
            Position = 2,
            ValueFromPipeline = false)]
        public string ReferenceAttribute { get; set; }

        [Parameter(
           Mandatory = true,
           HelpMessage = "The name of the provisioning control attribute",
           Position = 3,
           ValueFromPipeline = false)]
        public string ProvisioningAttribute { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                AcmaSchemaObjectClass shadowClass = ActiveConfig.DB.GetObjectClass(this.ShadowObjectClass);

                if (!shadowClass.IsShadowObject)
                {
                    throw new ArgumentException("The specified object class is not a shadow object class");
                }

                AcmaSchemaObjectClass parentClass = shadowClass.ShadowFromObjectClass;
                AcmaSchemaAttribute provisioningAttribute = ActiveConfig.DB.GetAttribute(this.ProvisioningAttribute, parentClass);
                AcmaSchemaAttribute referenceAttribute = ActiveConfig.DB.GetAttribute(this.ReferenceAttribute, parentClass);

                ActiveConfig.DB.CreateShadowLink(shadowClass, provisioningAttribute, referenceAttribute, this.Name);
                ActiveConfig.DB.ClearCache();

            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
        }
    }
}
