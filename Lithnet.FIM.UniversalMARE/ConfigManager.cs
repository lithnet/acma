using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;
using Lithnet.Fim.Transforms;
using Lithnet.Fim.Core;
using System.Configuration;

namespace Lithnet.Fim.UniversalMARE
{
    public static class ConfigManager
    {
        private static Configuration appConfig;

        public static string AppConfigPath
        {
            get
            {
                KeyValueConfigurationElement element = ConfigManager.AppConfig.AppSettings.Settings["ConfigFile"];
                if (element != null)
                {
                    string value = element.Value;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }

                return string.Empty;
            }
        }

        public static Configuration AppConfig
        {
            get
            {
                if (ConfigManager.appConfig == null)
                {
                    Uri exeConfigUri = new Uri(typeof(ConfigManager).Assembly.CodeBase);
                    ConfigManager.appConfig = ConfigurationManager.OpenExeConfiguration(exeConfigUri.LocalPath);
                }

                return ConfigManager.appConfig;
            }
        }

        public static XmlConfigFile LoadXml(string filename)
        {
            UniqueIDCache.ClearIdCache();

            FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());

            DataContractSerializer serializer = new DataContractSerializer(typeof(XmlConfigFile));
            XmlConfigFile configFile = (XmlConfigFile)serializer.ReadObject(xdr);

            configFile.FileName = filename;
            configFile.ResetChangeState();

            return configFile;
        }

        public static void Save(string filename, XmlConfigFile configFile)
        {
            if (configFile == null)
            {
                throw new ArgumentNullException("configFile");
            }

            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.IndentChars = "  ";
                writerSettings.NewLineChars = Environment.NewLine;
                writerSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

                XmlWriter writer = XmlWriter.Create(stream, writerSettings);

                DataContractSerializer serializer = new DataContractSerializer(typeof(XmlConfigFile));
                serializer.WriteObject(writer, configFile);

                writer.Flush();
                writer.Close();
            }

            configFile.FileName = filename;
            configFile.ResetChangeState();
        }
    }
}
