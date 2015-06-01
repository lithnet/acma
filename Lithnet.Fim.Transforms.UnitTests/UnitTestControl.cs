using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using System.Xml;
using Lithnet.Logging;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace Lithnet.Fim.Transforms.UnitTests
{
    public static class UnitTestControl
    {
        private static bool initialized = false;

        public static void Initialize()
        {
            if (!initialized)
            {
                Lithnet.Fim.MmsAssemblyResolver.RegisterResolver();
                TransformGlobal.HostProcessSupportsLoopbackTransforms = true;
                TransformGlobal.HostProcessSupportsNativeDateTime = true;
                Logger.LogPath = @"D:\MAData\Lithnet.Fim.Transforms\lithnet.fim.transforms.unittests.log";
                Logger.LogLevel = LogLevel.Debug;
                initialized = true;
            }
        }

        public static T XmlSerializeRoundTrip<T>(object objectToSerialize)
        {
            string filename = Path.GetTempFileName();
            bool delete = true;

            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlWriterSettings writerSettings = new XmlWriterSettings();
                    writerSettings.Indent = true;
                    writerSettings.IndentChars = "  ";
                    writerSettings.NewLineChars = Environment.NewLine;

                    XmlWriter writer = XmlWriter.Create(stream, writerSettings);
                    writer.WriteStartDocument();
                    writer.WriteStartElement("acma-unit-tests");

                    writer.WriteStartElement("test-data");
                    writer.WriteAttributeString("id", "x" + Guid.NewGuid().ToString());

                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                    serializer.WriteObject(writer, objectToSerialize);

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                    stream.Position = 0;

                    XmlReaderSettings readerSettings = new XmlReaderSettings();
                    XmlReader reader = XmlReader.Create(stream, readerSettings);
                    reader.ReadStartElement();
                    reader.ReadStartElement();
                    T deserializedObject = (T)serializer.ReadObject(reader);
                    
                    reader.Close();
                    stream.Close();
                    stream.Dispose();

                    return deserializedObject;
                }
            }
            catch
            {
                delete = false;
                throw;
            }
            finally
            {
                if (delete && File.Exists(filename))
                {
                    System.Diagnostics.Debug.WriteLine(System.IO.File.ReadAllText(filename));
                    File.Delete(filename);
                }
            }
        }

        static void readerSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }
    }
}
