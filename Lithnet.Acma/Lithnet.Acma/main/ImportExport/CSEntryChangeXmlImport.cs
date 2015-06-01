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
    public static class CSEntryChangeXmlImport
    {
        public static AcmaCSEntryChange ImportFromXml(XElement element)
        {
            AcmaCSEntryChange csentry = new AcmaCSEntryChange();
            CSEntryChangeXmlImport.ImportFromXml(element, csentry);
            return csentry;
        }

        public static void ImportFromXml(XElement element, CSEntryChange csentry)
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
        }

        private static void XmlReadAttributeChangesNode(XElement element, CSEntryChange csentry)
        {
            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "attribute-change"))
            {
                CSEntryChangeXmlImport.XmlReadAttributeChangeNode(child, csentry);
            }
        }

        private static void XmlReadAnchorAttributesNode(XElement element, CSEntryChange csentry)
        {
            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "anchor-attribute"))
            {
                CSEntryChangeXmlImport.XmlReadAnchorAttributeNode(child, csentry);
            }
        }

        private static void XmlReadAnchorAttributeNode(XElement element, CSEntryChange csentry)
        {
            string name = null;
            string value = null;

            foreach (var child in element.Elements())
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
            ExtendedAttributeType dataType = ExtendedAttributeType.Undefined;
            List<ValueChange> valueChanges = null;
            AttributeChange attributeChange = null;

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "name")
                {
                    name = (string)child;
                }
                else if (child.Name.LocalName == "modification-type")
                {
                    string modificationTypeString = (string)child;

                    if (!Enum.TryParse<AttributeModificationType>(modificationTypeString, out modificationType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(AttributeModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "data-type")
                {
                    string dataTypeString = (string)child;

                    if (!Enum.TryParse<ExtendedAttributeType>(dataTypeString, out dataType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type '{1}'", dataTypeString, typeof(ExtendedAttributeType).Name));
                    }
                }
                else if (child.Name.LocalName == "value-changes")
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new ArgumentException("The attribute name must appear first in the list of <attribute-change> elements");
                    }

                    if (dataType == ExtendedAttributeType.Undefined)
                    {
                        if (ActiveConfig.DB == null)
                        {
                            throw new NotConnectedException("The CSEntryChange did not specify a data type in the attribute change, and there was no active connection to the database to resolve it internally");
                        }
                        else
                        {
                            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(name);
                            dataType = attribute.Type;
                        }
                    }

                    valueChanges = CSEntryChangeXmlImport.GetValueChanges(child, dataType);
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
                ValueChange change = CSEntryChangeXmlImport.GetValueChange(child, attributeType);
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
    }
}
