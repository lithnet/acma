using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Lithnet.Logging;
using System.IO;
using Microsoft.MetadirectoryServices;
using System.Configuration;
using System.Xml;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using Lithnet.Acma.DataModel;
using System.ComponentModel;

namespace Lithnet.Acma
{
    public static class ActiveConfig
    {
        public static event EventHandler DatabaseConnectionChanged;

        public static event EventHandler XmlConfigChanged;

        /// <summary>
        /// Defines the default commit depth value
        /// </summary>
        private static int defaultCommitDepth = 20;

        /// <summary>
        /// Defines the default unique allocate depth value
        /// </summary>
        private static int defaultUniqueAllocationDepth = 10000;

        static ActiveConfig()
        {
            ActiveConfig.ReadRegistrySettings();
            ActiveConfig.XmlConfig = new XmlConfigFile();
        }

        public static AcmaDatabase DB { get; set; }

        public static XmlConfigFile XmlConfig { get; set; }

        public static void OpenDatabase(string server, string database)
        {
            ActiveConfig.DB = new AcmaDatabase(string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", server, database));
            ActiveConfig.DB.CanCache = true;

            if (ActiveConfig.DatabaseConnectionChanged != null)
            {
                ActiveConfig.DatabaseConnectionChanged(null, new EventArgs());
            }
        }

        /// <summary>
        /// Gets the maximum level of recursion allowed by a commit operation on an object before throwing an exception
        /// </summary>
        public static int CommitDepth { get; private set; }

        /// <summary>
        /// Gets the maximum number of attempts that can be made to make an attribute value unique
        /// </summary>
        public static int UniqueAllocationDepth { get; private set; }

        /// <summary>
        /// Reads the MAs registry settings
        /// </summary>
        private static void ReadRegistrySettings()
        {
            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey key = baseKey.OpenSubKey("Software\\Lithnet\\ACMA");
            if (key != null)
            {
                ActiveConfig.CommitDepth = (int)key.GetValue("CommitDepth", defaultCommitDepth);
                ActiveConfig.UniqueAllocationDepth = (int)key.GetValue("UniqueAllocationDepth", defaultUniqueAllocationDepth);
                if ((int)key.GetValue("RuleDebugging", 0) == 1)
                {
                    Rule.RuleFailedEvent += RuleBase_RuleFailedEvent;
                    RuleGroup.RuleGroupFailedEvent += RuleGroup_RuleGroupFailedEvent;
                }
            }
            else
            {
                ActiveConfig.CommitDepth = defaultCommitDepth;
                ActiveConfig.UniqueAllocationDepth = defaultUniqueAllocationDepth;
            }
        }

        private static void RuleGroup_RuleGroupFailedEvent(RuleGroup sender, string failureReason)
        {
            Logger.WriteLine("Rule group {0} failed evaluation: {1}", sender.ToString(), failureReason);
        }

        private static void RuleBase_RuleFailedEvent(Rule sender, string failureReason)
        {
            Logger.WriteLine("Rule {0} failed evaluation: {1}", sender.ToString(), failureReason);
        }

        public static XmlConfigFile LoadXml(string filename)
        {
            if (!Path.IsPathRooted(filename))
            {
                filename = Path.Combine(Utils.ExtensionsDirectory, filename);
            }

            bool canCache = ActiveConfig.DB.CanCache;

            try
            {
                UniqueIDCache.ClearIdCache();
                ActiveConfig.DB.CanCache = true;
                // XmlConfigFile configFile = Serializer.Read<XmlConfigFile>(filename);
                XmlConfigFile configFile = Serializer.Read<XmlConfigFile>(filename, "acma", "http://lithnet.local/Lithnet.Acma/v1/");

                configFile.FileName = filename;
                ActiveConfig.XmlConfig = configFile;

                if (ActiveConfig.XmlConfigChanged != null)
                {
                    ActiveConfig.XmlConfigChanged(null, new EventArgs());
                }
            }
            finally
            {
                ActiveConfig.DB.CanCache = canCache;
            }

            return ActiveConfig.XmlConfig;
        }
    }
}