using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;

namespace Lithnet.Acma.Service
{
    public class AcmaWCF : IAcmaWCF
    {
        public AcmaCSEntryChange GetObject(Guid ID)
        {
            MAObjectHologram hologram = ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(ID);

            if (hologram == null)
            {
                return null;
            }

            return hologram.CreateCSEntryChangeFromMAObjectHologram();
        }

        public AcmaCSEntryChange[] GetObjects(DBQueryObject query)
        {
            List<AcmaCSEntryChange> results = new List<AcmaCSEntryChange>();

            foreach (MAObjectHologram hologram in ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query))
            {
                results.Add(hologram.CreateCSEntryChangeFromMAObjectHologram());
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
    }
}
