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
        /// The largest page size GetResources accepts
        /// </summary>
        public const int MaximumPageSize = 1000;
    }
}
