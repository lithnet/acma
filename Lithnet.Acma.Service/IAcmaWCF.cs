using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lithnet.Acma;

namespace Lithnet.Acma.Service
{
    [ServiceContract]
    public interface IAcmaWCF
    {
        [OperationContract]
        AcmaCSEntryChange GetObject(Guid ID);

        [OperationContract]
        AcmaCSEntryChange[] GetObjects(DBQueryObject query);

        [OperationContract]
        void ExportObject(AcmaCSEntryChange csentry);
    }
}
