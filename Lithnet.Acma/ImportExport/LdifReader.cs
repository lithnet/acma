using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;

namespace Lithnet.Acma
{
    public static class LdifReader
    {
        private static int LineNumber = 0;

        public static CSEntryChange GetCSEntryChangeFromLdif(TextReader reader)
        {
            string line;

            line = GetNextDataRow(reader, false, false);

            if (!line.StartsWith("version:", StringComparison.OrdinalIgnoreCase))
            {
                throw new LdifFormatException(string.Format("A {0} value was expected when a {1} was found", "version", line));
            }

            LdifAttributeValuePair version = new LdifAttributeValuePair(line);
            if (version.Value.ToString() != "1")
            {
                throw new LdifFormatException("The LDIF file format version is unknown");
            }

            line = GetNextDataRow(reader, false, false);
            LdifAttributeValuePair dn = new LdifAttributeValuePair(line);
            if (dn.Name != "dn" || dn.Value == null)
            {
                throw new LdifFormatException(string.Format("A {0} value was expected when a {1} was found", "dn", dn.Name));
            }

            LdifAttributeValuePair element = new LdifAttributeValuePair(GetNextDataRow(reader, false, false));
            if (element.Name.Equals("control", StringComparison.OrdinalIgnoreCase) || element.Name.Equals("changetype", StringComparison.OrdinalIgnoreCase))
            {
                // This is an object modification
                if (element.Name.Equals("control", StringComparison.OrdinalIgnoreCase))
                {
                    element = new LdifAttributeValuePair(GetNextDataRow(reader, false, false));
                    if (!element.Name.Equals("changetype", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new LdifFormatException(string.Format("A {0} value was expected when a {1} was found", "changetype", element.Name));
                    }
                }

                return GetCSEntryChangeFromLdifChange(reader, dn, element);
            }
            else
            {
                return GetCSEntryChangeFromLdifRecord(reader, dn, element);
            }
        }

        internal static string GetNextDataRow(TextReader reader, bool returnEmptyLines, bool returnComments)
        {
            StringBuilder builder = new StringBuilder();

            string line = null;

            do
            {
                line = reader.ReadLine();
                LineNumber++;
                if (returnEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    return null;
                }
            }
            while (string.IsNullOrWhiteSpace(line));

            builder.Append(line);

            while (reader.Peek() == (int)' ')
            {
                line = reader.ReadLine();
                LineNumber++;
                line = line.Remove(0, 1);
                builder.Append(line);
            }

            string result = builder.ToString();
            if (result.StartsWith("#") && !returnComments)
            {
                return GetNextDataRow(reader, returnEmptyLines, returnComments);
            }
            else
            {
                return result;
            }
        }

        internal static IEnumerable<LdifAttributeValuePair> GetRemainingValuePairsInEntry(TextReader reader)
        {
            string line;

            line = GetNextDataRow(reader, true, false);
            List<LdifAttributeValuePair> pairs = new List<LdifAttributeValuePair>();

            while (line != null)
            {
                LdifAttributeValuePair pair = new LdifAttributeValuePair(line);
                if (pair.Name != null)
                {
                    pairs.Add(pair);
                }

                line = GetNextDataRow(reader, true, false);
            }

            return pairs;
        }

        private static CSEntryChange GetCSEntryChangeFromLdifChange(TextReader reader, LdifAttributeValuePair dn, LdifAttributeValuePair changeType)
        {
            CSEntryChange csentry = CSEntryChangeDetached.Create();
            csentry.AnchorAttributes.Add(AnchorAttribute.Create("ldifdn", dn.Value));
            string modificationType = changeType.Value as string;

            switch (modificationType)
            {
                case "delete":
                    csentry.ObjectModificationType = ObjectModificationType.Delete;
                    return csentry;

                case "add":
                    csentry.ObjectModificationType = ObjectModificationType.Add;
                    List<LdifAttributeValuePair> results = GetRemainingValuePairsInEntry(reader).ToList();
                    PopulateAttributeAddsIntoCSEntryChange(dn, csentry, results);
                    break;

                case "modify":
                    csentry.ObjectModificationType = ObjectModificationType.Update;
                    break;
                default:
                    break;
            }

            return csentry;
        }

        private static CSEntryChange GetCSEntryChangeFromLdifRecord(TextReader reader, LdifAttributeValuePair dn, LdifAttributeValuePair currentEntry)
        {
            CSEntryChange csentry = CSEntryChangeDetached.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;

            csentry.AnchorAttributes.Add(AnchorAttribute.Create("ldifdn", dn.Value));
            List<LdifAttributeValuePair> results = GetRemainingValuePairsInEntry(reader).ToList();
            results.Insert(0, currentEntry);

            PopulateAttributeAddsIntoCSEntryChange(dn, csentry, results);

            return csentry;
        }

        private static void PopulateAttributeAddsIntoCSEntryChange(LdifAttributeValuePair dn, CSEntryChange csentry, List<LdifAttributeValuePair> results)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (string name in results.Select(t => t.Name).Distinct())
            {
                List<object> values = new List<object>();
                foreach (object value in results.Where(u => u.Name == name).Select(t => t.Value))
                {
                    values.Add(value);
                }

                if (name.Equals("objectclass", StringComparison.OrdinalIgnoreCase))
                {
                    string objectClass = values.FirstOrDefault(t => ActiveConfig.DB.HasObjectClass(t.ToString())) as string;
                    if (objectClass == null)
                    {
                        throw new LdifFormatException(string.Format("The object class for object '{0}' was unknown", dn.Value));
                    }
                    else
                    {
                        csentry.ObjectType = objectClass;
                    }

                    continue;
                }

                AttributeChange change = AttributeChange.CreateAttributeAdd(name, values);
                csentry.AttributeChanges.Add(change);
            }
        }

        private static void PopulateAttributeChangesIntoCSEntryChange(LdifAttributeValuePair dn, CSEntryChange csentry, TextReader reader)
        {
            IList<string> lines = GetLinesInBlock(reader);
            IList<LdifAttributeValuePair> pairs = lines.Select(t => new LdifAttributeValuePair(t)).ToList();

            do
            {
                LdifAttributeValuePair modTypePair = pairs.First();
                ActiveConfig.DB.ThrowOnMissingAttribute(modTypePair.Value.ToString());
                IList<object> values = GetAttributeValues(pairs, modTypePair);
                string attributeName = modTypePair.Value.ToString();

                if (modTypePair.Name.Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    if (values.Count == 0)
                    {
                        throw new LdifFormatException("The attribute 'add' operation did not contain any values at line: " +  LineNumber);
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, values.Select(t => ValueChange.CreateValueAdd(t)).ToList()));
                    }
                }
                else if (modTypePair.Name.Equals("delete", StringComparison.OrdinalIgnoreCase))
                {
                    if (values.Count == 0)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, values.Select(t => ValueChange.CreateValueDelete(t)).ToList()));
                    }
                }
                else if (modTypePair.Name.Equals("replace"))
                {
                    if (values.Count == 0)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, values));
                    }
                }


                //    case "modify":
                //    case "modrdn":
                //    case "moddn":

                lines = GetLinesInBlock(reader);

            } while (reader.Peek() > -1);
        }

        private static IList<object> GetAttributeValues(IList<LdifAttributeValuePair> pairs, LdifAttributeValuePair modTypePair)
        {
            List<object> values = new List<object>();

            foreach (LdifAttributeValuePair valuePair in pairs.Where(t => t != modTypePair))
            {
                if (valuePair.Name != modTypePair.Value.ToString())
                {
                    throw new LdifFormatException(string.Format("The value {1} was unexpected: Line: {0}", LineNumber, valuePair.Name));
                }

                values.Add(valuePair.Value);
            }

            return values;
        }


        private static void GetValues(IList<string> lines)
        {

        }

        private static IList<string> GetLinesInBlock(TextReader reader)
        {
            List<string> lines = new List<string>();
            string line = GetNextDataRow(reader, true, false);

            while (line != "-" && line != null)
            {
                lines.Add(line);
                line = GetNextDataRow(reader, true, false);
            }

            return lines;
        }
    }
}
