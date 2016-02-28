using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.Service
{
    [KnownType(typeof(MAObjectHologram))]
    [KnownType(typeof(List<string>))]
    public class AcmaWCF : IAcmaWCF
    {
        public MAObjectHologram Get(string ID)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(new Guid(ID));
        }

        public CSEntryChange GetCSEntryChange(string ID)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectOrDefault(new Guid(ID)).CreateCSEntryChangeFromMAObjectHologram();
        }

        public CSEntryChange GetCSEntryChange(string objectType, string key, string keyValue)
        {
            DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), ValueOperator.Equals, keyValue);
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query).FirstOrDefault().CreateCSEntryChangeFromMAObjectHologram();
        }

        public MAObjectHologram GetResourceByKey(string objectType, string key, string keyValue)
        {
            DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), ValueOperator.Equals, keyValue);
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query).FirstOrDefault();
        }

        public string GetCurrentWatermark()
        {
            return ActiveConfig.DB.MADataConext.GetHighWatermarkMAObjects().ToSmartStringOrNull();
        }

        public MAObjectHologram[] GetObjects(string watermark)
        {
            byte[] byteWaterMark = null;

            if (watermark != null)
            {
                byteWaterMark = Convert.FromBase64String(watermark);
            }

            return ActiveConfig.DB.MADataConext.GetMAObjects(byteWaterMark).ToArray();
        }

        public MAObjectHologram[] GetObjectsByClass(string objectType)
        {
            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(ActiveConfig.DB.GetAttribute("objectClass"), ValueOperator.Equals, objectType).ToArray();
        }
    }
}
