using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace Lithnet.Acma
{
    [Serializable]
    public class AcmaCSEntryChange : CSEntryChangeDetached, IXmlSerializable
    {
        public AcmaCSEntryChange()
            : base(Guid.NewGuid(), ObjectModificationType.Unconfigured, MAImportError.Success, null)
        {
        }

        public AcmaCSEntryChange(CSEntryChange csentry)
            : this()
        {
            AcmaCSEntryChange.CloneCSEntryChange(csentry, this);
        }

        private static void CloneCSEntryChange(CSEntryChange csentry, AcmaCSEntryChange acmaCSEntry)
        {
            acmaCSEntry.DN = csentry.DN;
            acmaCSEntry.ErrorCodeImport = csentry.ErrorCodeImport;
            acmaCSEntry.ErrorDetail = csentry.ErrorDetail;
            acmaCSEntry.ErrorName = csentry.ErrorName;
            acmaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            acmaCSEntry.ObjectType = csentry.ObjectType;

            foreach (var item in csentry.AnchorAttributes)
            {
                acmaCSEntry.AnchorAttributes.Add(item);
            }

            foreach (var item in csentry.AttributeChanges)
            {
                acmaCSEntry.AttributeChanges.Add(item);
            }
        }

        public static AcmaCSEntryChange FromCSEntryChange(CSEntryChange csentry)
        {
            AcmaCSEntryChange obj = new AcmaCSEntryChange();

            AcmaCSEntryChange.CloneCSEntryChange(csentry, obj);

            return obj;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var doc = XDocument.Load(reader.ReadSubtree());

            XElement element = doc.Root.Elements().FirstOrDefault();

            if (element == null)
            {
                throw new ArgumentException("The XML stream did not contain the object-change element");
            }

            CSEntryChangeXmlImport.ImportFromXml(element, this, false);
        }

        public void WriteXml(XmlWriter writer)
        {
            CSEntryChangeXmlExport.ExportToXml(this, writer);
        }
    }
}