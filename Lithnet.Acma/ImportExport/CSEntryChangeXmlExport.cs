using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using Microsoft.MetadirectoryServices.DetachedObjectModel;

namespace Lithnet.Acma
{
    public static class CSEntryChangeXmlExport
    {
        public static void ExportToXml(CSEntryChange csentry, XmlWriter writer)
        {
            writer.WriteStartElement("object-change");
            writer.WriteElementString("modification-type", csentry.ObjectModificationType.ToString());
            writer.WriteElementString("id", csentry.DN);
            writer.WriteElementString("object-class", csentry.ObjectType);

            if (csentry.AnchorAttributes.Count > 0)
            {
                writer.WriteStartElement("anchor-attributes");
                foreach (AnchorAttribute anchor in csentry.AnchorAttributes)
                {
                    writer.WriteStartElement("anchor-attribute");
                    writer.WriteElementString("name", anchor.Name);
                    writer.WriteElementString("value", anchor.Value.ToSmartStringOrNull());
                    writer.WriteEndElement(); // </anchor-attribute>
                }
                writer.WriteEndElement(); // </anchor-attributes>
            }

            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                case ObjectModificationType.Update:
                    XmlWriteAttributeChangesNode(csentry, writer);
                    break;

                case ObjectModificationType.Delete:
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(csentry.ObjectModificationType);
            }

            writer.WriteEndElement(); // </object-change>
        }

        public static void ExportToXml(MAObjectHologram maObject, XmlWriter writer)
        {
            CSEntryChangeXmlExport.ExportToXml(maObject, writer, ObjectModificationType.Add);
        }

        public static void ExportToXml(MAObjectHologram maObject, XmlWriter writer, ObjectModificationType modificationType)
        {
            CSEntryChange csentry = CSEntryChangeExtensions.CreateCSEntryChangeFromMAObjectHologram(maObject, modificationType);
            ExportToXml(csentry, writer);
        }

        public static void ExportToXml(IEnumerable<MAObjectHologram> maObjects, string fileName, ObjectModificationType modificationType)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = Environment.NewLine;
            XmlWriter writer = XmlWriter.Create(fileName, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("acma-export");

            CSEntryChangeXmlExport.ExportToXml(maObjects, writer, modificationType);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        public static void ExportToXml(IEnumerable<MAObjectHologram> maObjects, XmlWriter writer, ObjectModificationType modificationType)
        {
            foreach (MAObjectHologram maObject in maObjects)
            {
                CSEntryChangeXmlExport.ExportToXml(maObject, writer, modificationType);
            }
        }

        private static void XmlWriteAttributeChangesNode(CSEntryChange csentry, XmlWriter writer)
        {
            writer.WriteStartElement("attribute-changes");

            foreach (AttributeChange attributeChange in csentry.AttributeChanges)
            {
                writer.WriteStartElement("attribute-change");
                writer.WriteElementString("name", attributeChange.Name);
                writer.WriteElementString("modification-type", attributeChange.ModificationType.ToString());

                if (ActiveConfig.DB != null)
                {
                    AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(attributeChange.Name);
                    writer.WriteElementString("data-type", attribute.Type.ToString());
                }
                else
                {
                    writer.WriteElementString("data-type", ExtendedAttributeType.Undefined.ToString());
                }

                writer.WriteStartElement("value-changes");

                foreach (ValueChange valueChange in attributeChange.ValueChanges)
                {
                    writer.WriteStartElement("value-change");
                    writer.WriteElementString("modification-type", valueChange.ModificationType.ToString());
                    writer.WriteElementString("value", valueChange.Value.ToSmartString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // </value-changes>
                writer.WriteEndElement(); // </attribute-change>
            }

            writer.WriteEndElement(); // </attribute-changes>
        }
    }
}
