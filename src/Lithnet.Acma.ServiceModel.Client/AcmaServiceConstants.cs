using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma.ServiceModel
{
    public class AcmaServiceConstants
    {
        public const string ResourcesNamespace = "http://lithnet.local/acma/resource/";
        public const string SchemaNamespace = "http://lithnet.local/acma/schema/";

        /// <summary>
        /// The largest page size GetResources and GetResourcesPage accept
        /// </summary>
        public const int MaximumPageSize = 1000;

        /// <summary>
        /// The number of minutes after its last page was read that a read started by GetResources ends
        /// </summary>
        public const int ResourceReadExpiryMinutes = 10;
    }
}
