using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using System.Xml;
using Lithnet.Logging;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.ComponentModel;

namespace Lithnet.Acma.UnitTests
{
    public static class UnitTestControl
    {
        private static bool initialized = false;

        public static void Initialize()
        {
            if (!initialized)
            {
                Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();
                Logger.LogPath = @"D:\MAData\ACMA\acma-unit-test.log";
                Logger.LogLevel = LogLevel.Debug;
                XmlDocument doc = new XmlDocument();
                ActiveConfig.OpenDatabase("localhost", "AcmaUnitTestDB2");
                LoadTestTransforms();
                initialized = true;
            }
        }

        public static void DeleteAllMAObjects()
        {
            byte[] watermark = MAObjectHologram.GetHighWatermarkMAObjects();
            foreach (MAObjectHologram hologram in MAObjectHologram.GetMAObjects(watermark))
            {
                MAObjectHologram.DeleteMAObjectPermanent(hologram.ObjectID);
            }
        }

        private static void LoadTestTransforms()
        {
            StringCaseTransform t1 = new StringCaseTransform();
            t1.ID = "ToUpperCase";
            t1.StringCase = StringCaseType.Upper;
            ActiveConfig.XmlConfig.Transforms.Add(t1);

            StringCaseTransform t2 = new StringCaseTransform();
            t2.ID = "ToLowerCase";
            t2.StringCase = StringCaseType.Lower;
            ActiveConfig.XmlConfig.Transforms.Add(t2);

            SubstringTransform t3 = new SubstringTransform();
            t3.ID = "GetFirstCharacter";
            t3.Length = 1;
            ActiveConfig.XmlConfig.Transforms.Add(t3);

            DateConverterTransform t4 = new DateConverterTransform();
            t4.ID = "Add30Days";
            t4.InputDateType = DateType.Ticks;
            t4.InputTimeZone = TimeZoneInfo.Utc;
            t4.OutputDateType = DateType.Ticks;
            t4.OutputTimeZone = TimeZoneInfo.Utc;
            t4.CalculationOperator = DateOperator.Add;
            t4.CalculationTimeSpanType = TimeSpanType.Days;
            t4.CalculationValue = 30;
            ActiveConfig.XmlConfig.Transforms.Add(t4);
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
