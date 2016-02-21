using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using Lithnet.Acma.DataModel;
using Microsoft.MetadirectoryServices.DetachedObjectModel;

namespace Lithnet.Acma
{
    public static class ImportExportEngine
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

        public static void ExportToXml(MAObjectHologram maObject, XmlWriter writer, ObjectModificationType modificationType)
        {
            CSEntryChange csentry = CSEntryChangeExtensions.CreateCSEntryChangeFromMAObjectHologram(maObject, modificationType);
            ExportToXml(csentry, writer);
        }

        public static void ExportToXml(IEnumerable<MAObjectHologram> maObjects, string fileName, ObjectModificationType  modificationType)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = Environment.NewLine;
            XmlWriter writer = XmlWriter.Create(fileName, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("acma-export");

            ImportExportEngine.ExportToXml(maObjects, writer, modificationType);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        public static void ExportToXml(IEnumerable<MAObjectHologram> maObjects, XmlWriter writer, ObjectModificationType modificationType)
        {
            foreach (MAObjectHologram maObject in maObjects)
            {
                ImportExportEngine.ExportToXml(maObject, writer, modificationType);
            }
        }

        public static CSEntryChange ImportFromXml(XElement element)
        {
            CSEntryChange csentry = CSEntryChange.Create();
            return ImportExportEngine.ImportFromXml(element, csentry);
        }

        public static CSEntryChange ImportFromXml(XElement element, CSEntryChange csentry)
        {
            if (element.Name.LocalName != "object-change")
            {
                throw new ArgumentException("The XML node provided was not an <object-change> node", "node");
            }

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "modification-type")
                {
                    ObjectModificationType modificationType;
                    string modificationTypeString = (string)child;

                    if (Enum.TryParse<ObjectModificationType>(modificationTypeString, out modificationType))
                    {
                        csentry.ObjectModificationType = modificationType;
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(ObjectModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "object-class")
                {
                    csentry.ObjectType = (string)child;
                }
                else if (child.Name.LocalName == "id")
                {
                    csentry.DN = (string)child;
                }
                else if (child.Name.LocalName == "attribute-changes")
                {
                    XmlReadAttributeChangesNode(child, csentry);
                }
                else if (child.Name.LocalName == "anchor-attributes")
                {
                    XmlReadAnchorAttributesNode(child, csentry);
                }
            }

            return csentry;
        }

        private static void XmlReadAttributeChangesNode(XElement element, CSEntryChange csentry)
        {
            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "attribute-change"))
            {
                ImportExportEngine.XmlReadAttributeChangeNode(child, csentry);
            }
        }

        private static void XmlReadAnchorAttributesNode(XElement element, CSEntryChange csentry)
        {
           foreach(var child in element.Elements().Where(t => t.Name.LocalName == "anchor-attribute"))
           {
                ImportExportEngine.XmlReadAnchorAttributeNode(child, csentry);
           }
        }

        private static void XmlReadAnchorAttributeNode(XElement element, CSEntryChange csentry)
        {
            string name = null;
            string value = null;

            foreach(var child in element.Elements())
            {
                if (child.Name.LocalName == "name")
                {
                    name = (string)child;
                }
                else if (child.Name.LocalName == "value")
                {
                    value = (string)child;
                }
            }

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(value))
            {
                return;
            }


            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The name and value elements of an <anchor-attribute> must not be null");
            }

            AnchorAttribute anchor = AnchorAttribute.Create(name, value);

            csentry.AnchorAttributes.Add(anchor);
        }

        private static void XmlReadAttributeChangeNode(XElement element, CSEntryChange csentry)
        {
            string name = null;
            AttributeModificationType modificationType = AttributeModificationType.Unconfigured;
            AcmaSchemaAttribute attribute = null;
            List<ValueChange> valueChanges = null;
            AttributeChange attributeChange = null;

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "name")
                {
                    name = (string)child;
                    attribute = ActiveConfig.DB.GetAttribute(name);
                }
                else if (child.Name.LocalName == "modification-type")
                {
                    string modificationTypeString = (string)child;

                    if (!Enum.TryParse<AttributeModificationType>(modificationTypeString, out modificationType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(AttributeModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "value-changes")
                {
                    if (attribute == null)
                    {
                        throw new ArgumentException("The attribute name must appear first in the list of <attribute-change> elements");
                    }

                    valueChanges = ImportExportEngine.GetValueChanges(child, attribute.Type);
                }
            }

            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (valueChanges.Count == 0)
                    {
                        // discard attribute change with no values
                        return;
                    }

                    attributeChange = AttributeChange.CreateAttributeAdd(name, valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    break;

                case AttributeModificationType.Replace:
                    if (valueChanges.Count == 0)
                    {
                        // discard attribute change with no values
                        return;
                        //throw new ArgumentException("The attribute replace in the CSEntry provided no values");
                    }

                    attributeChange = AttributeChange.CreateAttributeReplace(name, valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    break;

                case AttributeModificationType.Delete:
                    attributeChange = AttributeChange.CreateAttributeDelete(name);
                    break;

                case AttributeModificationType.Update:
                    if (valueChanges.Count == 0)
                    {
                        // discard attribute change with no values
                        return;
                        //throw new ArgumentException("The attribute update in the CSEntry provided no values");
                    }

                    attributeChange = AttributeChange.CreateAttributeUpdate(name, valueChanges);

                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(modificationType);
            }

            csentry.AttributeChanges.Add(attributeChange);

        }

        private static List<ValueChange> GetValueChanges(XElement element, ExtendedAttributeType attributeType)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "value-change"))
            {
                ValueChange change = ImportExportEngine.GetValueChange(child, attributeType);
                if (change != null)
                {
                    valueChanges.Add(change);
                }
            }

            return valueChanges;
        }

        private static ValueChange GetValueChange(XElement element, ExtendedAttributeType attributeType)
        {
            ValueModificationType modificationType = ValueModificationType.Unconfigured;
            string value = null;


            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "modification-type")
                {
                    string modificationTypeString = (string)child;

                    if (!Enum.TryParse<ValueModificationType>(modificationTypeString, out modificationType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(ValueModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "value")
                {
                    value = (string)child;
                }
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The value-change value was blank");
            }

            switch (modificationType)
            {
                case ValueModificationType.Add:
                    return ValueChange.CreateValueAdd(TypeConverter.ConvertData(value, attributeType));

                case ValueModificationType.Delete:
                    return ValueChange.CreateValueDelete(TypeConverter.ConvertData(value, attributeType));

                case ValueModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(modificationType);
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