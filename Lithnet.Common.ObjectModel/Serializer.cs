using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Lithnet.Common.ObjectModel
{
    public static class Serializer
    {
        public static T Read<T>(string filename)
        {
            T deserialized;

            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas() { MaxStringContentLength = 20971520 });
                deserialized = Serializer.Read<T>(xdr);
                xdr.Close();
                stream.Close();
            }

            return deserialized;
        }

        public static T Read<T>(string filename, string nodeName, string nodeURI)
        {
            T deserialized;

            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas() { MaxStringContentLength = 20971520 });
                xdr.ReadToFollowing(nodeName, nodeURI);
                deserialized = Serializer.Read<T>(xdr);
                xdr.Close();
                stream.Close();
            }

            return deserialized;
        }

        public static T Read<T>(XmlDictionaryReader xdr)
        {
            T deserialized;

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            deserialized = (T)serializer.ReadObject(xdr);

            return deserialized;
        }

        public static void Save<T>(string filename, T obj)
        {
            Serializer.Save<T>(filename, obj, null);
        }

        public static void Save<T>(string filename, T obj, Dictionary<string, string> namespacePrefixes)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.IndentChars = "  ";
                writerSettings.NewLineChars = Environment.NewLine;
                writerSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

                XmlWriter writer = XmlWriter.Create(stream, writerSettings);

                DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                if (namespacePrefixes == null || namespacePrefixes.Count == 0)
                {
                    serializer.WriteObject(writer, obj);
                }
                else
                {
                    serializer.WriteStartObject(writer, obj);
                    foreach (KeyValuePair<string, string> prefix in namespacePrefixes)
                    {
                        writer.WriteAttributeString("xmlns", prefix.Key, null, prefix.Value);
                    }
                    serializer.WriteObjectContent(writer, obj);
                    serializer.WriteEndObject(writer);
                }

                writer.Flush();
                writer.Close();
            }
        }
    }
}
