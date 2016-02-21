using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Lithnet.Acma;

namespace Lithnet.Acma.WS
{
    [ServiceContract]
    public interface IAcmaObjectWebService
    {
        [OperationContract]
        AcmaCSEntryChange GetObject(Guid ID);

        [OperationContract]
        AcmaCSEntryChange[] GetObjects(DBQueryObject query);

        [OperationContract]
        void ExportObject(AcmaCSEntryChange csentry);

        [OperationContract]
        void ExportObjectWithEvents(AcmaCSEntryChange csentry, string[] events);

        [OperationContract]
        bool DoesAttributeValueExist(string attributeName, object attributeValue, Guid requestingObjectID);
    }
}
