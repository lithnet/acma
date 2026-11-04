using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using System.Runtime.Caching;
using Lithnet.Acma.DataModel;
using System.ServiceModel;
using Lithnet.Acma.ServiceModel;
using Lithnet.Logging;
using System.Threading;
using System.ServiceModel.Web;

namespace Lithnet.Acma.Service
{
    [ServiceBehavior(Namespace = AcmaServiceConstants.ResourcesNamespace)]
    public class AcmaService : IAcmaResourceService
    {
        internal static ManualResetEvent ConfigLock = new ManualResetEvent(true);

        public AcmaResource GetResourceById(string id)
        {
            MAObjectHologram hologram = ActiveConfig.DB.GetMAObjectOrDefault(new Guid(id));

            if (hologram != null)
            {
                return hologram.ToAcmaResource();
            }
            else
            {
                return null;
            }
        }

        public AcmaResource GetResourceByTypeAndKey(string objectType, string key, string keyValue)
        {
            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectClass"), ValueOperator.Equals, objectType));
            queryGroup.AddChildQueryObjects(new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), ValueOperator.Equals, keyValue));

            IList<MAObjectHologram> holograms = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

            if (holograms.Count == 0)
            {
                return null;
            }
            else if (holograms.Count > 1)
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Ambiguous);
            }
            else
            {
                return holograms.First().ToAcmaResource();
            }
        }

        public IList<AcmaResource> GetResourcesByAttributePair(string key, string keyValue, string op = "Equals")
        {
            ValueOperator vo;

            if (!string.IsNullOrWhiteSpace(op))
            {
                vo = (ValueOperator)Enum.Parse(typeof(ValueOperator), op, true);
            }
            else
            {
                vo = ValueOperator.Equals;
            }

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(new DBQueryByValue(ActiveConfig.DB.GetAttribute(key), vo, keyValue));

            IList<MAObjectHologram> holograms = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

            return holograms.Select(t => t.ToAcmaResource()).ToList();
        }

        public IList<AcmaResource> GetResourcesByAttributePairs(string key1, string keyValue1, string key2, string keyValue2)
        {
            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(new DBQueryByValue(ActiveConfig.DB.GetAttribute(key1), ValueOperator.Equals, keyValue1));
            queryGroup.AddChildQueryObjects(new DBQueryByValue(ActiveConfig.DB.GetAttribute(key2), ValueOperator.Equals, keyValue2));

            IList<MAObjectHologram> holograms = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

            return holograms.Select(t => t.ToAcmaResource()).ToList();
        }

        public AcmaResourcePage GetResources(string objectType, IList<string> attributes, int pageSize, string continuationToken)
        {
            if (string.IsNullOrWhiteSpace(objectType))
            {
                throw new ArgumentNullException("objectType");
            }

            if (pageSize < 1 || pageSize > AcmaServiceConstants.MaximumPageSize)
            {
                throw new ArgumentOutOfRangeException("pageSize", string.Format("The page size must be between 1 and {0}", AcmaServiceConstants.MaximumPageSize));
            }

            Guid afterObjectId = Guid.Empty;

            if (!string.IsNullOrEmpty(continuationToken))
            {
                if (!Guid.TryParse(continuationToken, out afterObjectId))
                {
                    throw new ArgumentException("The continuation token is not valid. Pass the ContinuationToken of the previous page, or null for the first page", "continuationToken");
                }
            }

            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(objectType);
            List<AcmaSchemaAttribute> schemaAttributes = new List<AcmaSchemaAttribute>();

            if (attributes != null)
            {
                foreach (string attributeName in attributes)
                {
                    schemaAttributes.Add(ActiveConfig.DB.GetAttribute(attributeName, objectClass));
                }
            }

            IList<MAObjectHologram> holograms = ActiveConfig.DB.GetMAObjectsPage(objectClass, afterObjectId, pageSize);

            AcmaResourcePage page = new AcmaResourcePage();
            page.Resources = holograms.Select(t => t.ToAcmaResource(schemaAttributes)).ToList();

            if (holograms.Count == pageSize)
            {
                page.ContinuationToken = holograms[holograms.Count - 1].ObjectID.ToString();
            }

            return page;
        }

        public void ReplaceResource(string id, AcmaResource resource)
        {
            try
            {
                Monitor.Enter(ServiceMain.Lock);
                CSEntryChange csentry = CSEntryChangeDetached.Create();
                csentry.ObjectModificationType = ObjectModificationType.Replace;
                csentry.DN = id;

                foreach (var item in resource.Attributes)
                {
                    csentry.AttributeChanges.Add(this.AvpToAttributeChange(item, AttributeModificationType.Add));
                }

                bool refRetry;
                CSEntryExport.PutExportEntry(csentry, out refRetry);
            }
            finally
            {
                Monitor.Exit(ServiceMain.Lock);
            }
        }

        public void PatchResource(string id, AcmaResource resource)
        {
            try
            {
                Monitor.Enter(ServiceMain.Lock);
                CSEntryChange csentry = CSEntryChangeDetached.Create();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.DN = id;

                foreach (var item in resource.Attributes)
                {
                    csentry.AttributeChanges.Add(this.AvpToAttributeChange(item, AttributeModificationType.Replace));
                }

                bool refRetry;
                CSEntryExport.PutExportEntry(csentry, out refRetry);
            }
            finally
            {
                Monitor.Exit(ServiceMain.Lock);
            }
        }

        public void DeleteResource(string id)
        {
            try
            {
                Monitor.Enter(ServiceMain.Lock);
                CSEntryChange csentry = CSEntryChangeDetached.Create();
                csentry.ObjectModificationType = ObjectModificationType.Delete;
                csentry.DN = id;

                bool refRetry;
                CSEntryExport.PutExportEntry(csentry, out refRetry);
            }
            finally
            {
                Monitor.Exit(ServiceMain.Lock);
            }

        }

        public void CreateResource(AcmaResource resource)
        {
            try
            {
                Monitor.Enter(ServiceMain.Lock);

                if (resource.ObjectID == Guid.Empty)
                {
                    resource.ObjectID = Guid.NewGuid();
                }

                CSEntryChange csentry = CSEntryChangeDetached.Create();
                csentry.ObjectModificationType = ObjectModificationType.Add;
                csentry.DN = resource.ObjectID.ToString();
                csentry.ObjectType = resource.ObjectType;

                foreach (var item in resource.Attributes)
                {
                    csentry.AttributeChanges.Add(this.AvpToAttributeChange(item, AttributeModificationType.Add));
                }

                bool refRetry;
                CSEntryExport.PutExportEntry(csentry, out refRetry);
            }
            finally
            {
                Monitor.Exit(ServiceMain.Lock);
            }
        }



        private AttributeChange AvpToAttributeChange(KeyValuePair<string, IList<string>> kvp, AttributeModificationType modificationType)
        {
            ExtendedAttributeType type = ActiveConfig.DB.GetAttribute(kvp.Key).Type;

            switch (modificationType)
            {
                case AttributeModificationType.Replace:
                    if (kvp.Value == null)
                    {
                        return AttributeChangeDetached.CreateAttributeDelete(kvp.Key);
                    }
                    else
                    {
                        return AttributeChangeDetached.CreateAttributeReplace(kvp.Key, kvp.Value.Select(t => TypeConverter.ConvertData(t, type)).ToList<object>());
                    }

                case AttributeModificationType.Add:
                    return AttributeChangeDetached.CreateAttributeAdd(kvp.Key, kvp.Value.Select(t => TypeConverter.ConvertData(t, type)).ToList<object>());

                case AttributeModificationType.Delete:
                    return AttributeChangeDetached.CreateAttributeDelete(kvp.Key);

                default:
                case AttributeModificationType.Update:
                case AttributeModificationType.Unconfigured:
                    throw new InvalidOperationException();
            }
        }
    }
}