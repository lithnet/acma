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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Acma.UnitTests
{
    public static class UnitTestControl
    {
        private static bool initialized = false;

        /// <summary>
        /// The SQL Server instance that holds the unit test database. Set with the ACMA_UNITTEST_SQLSERVER environment variable.
        /// </summary>
        public static string ServerName
        {
            get
            {
                return GetSetting("ACMA_UNITTEST_SQLSERVER", "localhost");
            }
        }

        /// <summary>
        /// The name of the unit test database. Set with the ACMA_UNITTEST_DATABASE environment variable.
        /// </summary>
        public static string DatabaseName
        {
            get
            {
                return GetSetting("ACMA_UNITTEST_DATABASE", "AcmaUnitTestDB2");
            }
        }

        /// <summary>
        /// The log file for the test run. Set with the ACMA_UNITTEST_LOGPATH environment variable; defaults to acma-unit-test.log in the test output folder.
        /// </summary>
        public static string LogPath
        {
            get
            {
                return GetSetting("ACMA_UNITTEST_LOGPATH", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "acma-unit-test.log"));
            }
        }

        public static void Initialize()
        {
            if (!initialized)
            {
                string logPath = UnitTestControl.LogPath;
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(logPath)));
                Logger.LogPath = logPath;
                Logger.LogLevel = LogLevel.Debug;
                ActiveConfig.OpenDatabase(UnitTestControl.ServerName, UnitTestControl.DatabaseName);
                LoadTestTransforms();
                initialized = true;
            }
        }

        private static string GetSetting(string environmentVariable, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(environmentVariable);

            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;
        }

        public static void DeleteAllMAObjects()
        {
            byte[] watermark = ActiveConfig.DB.GetHighWatermarkMAObjects();
            foreach (MAObjectHologram hologram in ActiveConfig.DB.GetMAObjects(watermark))
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(hologram.ObjectID);
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
