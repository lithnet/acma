using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Lithnet.Acma;
using Lithnet.Common.ObjectModel;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using System.Configuration;

namespace Lithnet.Acma.WS
{
    public class AcmaObjectWebService : IAcmaObjectWebService
    {
        public AcmaObjectWebService()
        {
            if (ActiveConfig.DB == null)
            {
                ActiveConfig.DB = new AcmaDatabase(ConfigurationManager.ConnectionStrings["acmadb"].ConnectionString);
            }

            if (ActiveConfig.XmlConfig == null)
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

        public void ExportObjectWithEvents(AcmaCSEntryChange csentry, string[] events)
        {
            throw new NotImplementedException();
        }
    }
}
