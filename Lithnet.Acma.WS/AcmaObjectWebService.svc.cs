using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using System.Configuration;
using Lithnet.Acma.DataModel;
using Lithnet.Logging;
using Lithnet.Acma.ServiceModel;

namespace Lithnet.Acma.WS
{
    public class AcmaObjectWebService : IAcmaObjectWebService
    {
        public AcmaObjectWebService()
        {
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();

            if (ActiveConfig.DB == null)
            {
                ActiveConfig.DB = new AcmaDatabase(ConfigurationManager.ConnectionStrings["acmadb"].ConnectionString);
            }

            if (ActiveConfig.XmlConfig == null || ActiveConfig.XmlConfig.FileName == null)
            {
                ActiveConfig.LoadXml(ConfigurationManager.AppSettings["configfile"]);
            }
        }

        public AcmaCSEntryChange GetObject(Guid ID)
        {
            MAObjectHologram hologram = ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(ID);

            if (hologram == null)
            {
                throw new NotFoundException();
            }

            return hologram.CreateCSEntryChangeFromMAObjectHologram(ObjectModificationType.Add);
        }

        public AcmaCSEntryChange[] GetObjects(DBQueryObject query)
        {
            List<AcmaCSEntryChange> results = new List<AcmaCSEntryChange>();

            foreach (MAObjectHologram hologram in ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query))
            {
                results.Add(hologram.CreateCSEntryChangeFromMAObjectHologram(ObjectModificationType.Add));
            }

            return results.ToArray();
        }

        public void ExportObject(AcmaCSEntryChange csentry)
        {
            bool refRetryRequired;
            CSEntryExport.PutExportEntry(csentry, ActiveConfig.DB.MADataConext, out refRetryRequired);

            if (refRetryRequired)
            {
                throw new ReferencedObjectNotPresentException();
            }
        }

        public bool DoesAttributeValueExist(string attributeName, object attributeValue, Guid requestingObjectID)
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(attributeName);

            return ActiveConfig.DB.MADataConext.DoesAttributeValueExist(attribute, attributeValue, requestingObjectID);
        }

        public void ExportObjectWithEvents(AcmaCSEntryChange csentry, string[] events)
        {
            throw new NotImplementedException();
        }
    }
}
