using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Microsoft.Win32;

namespace Lithnet.Acma.PS
{
    public class AcmaCmdletConnected : Cmdlet
    {
        private static bool isManuallyConnected;

        private static bool isAutoConnected;

        private static bool yesToAll;

        private static bool noToAll;

        protected bool IsConnectionStatusOk(bool mayRequireRules)
        {
            if (AcmaCmdletConnected.isManuallyConnected)
            {
                return true;
            }
            else if (!AcmaCmdletConnected.isAutoConnected)
            {
                if (this.TryAutoConnect())
                {
                    if (mayRequireRules)
                    {
                        return this.PromptForContinuePermission();
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    this.ThrowTerminatingError(new ErrorRecord(new NotConnectedException(), "Call Connect-AcmaEngine before using this cmdlet", ErrorCategory.OpenError, null));
                    return false;
                }
            }
            else
            {
                if (AcmaCmdletConnected.noToAll)
                {
                    this.ThrowTerminatingError(new ErrorRecord(new NotConnectedException(), "Call Connect-AcmaEngine before using this cmdlet", ErrorCategory.OpenError, null));
                    return false;
                }
                else if (AcmaCmdletConnected.yesToAll)
                {
                    return true;
                }
                else
                {
                    if (mayRequireRules)
                    {
                        return this.PromptForContinuePermission();
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        protected void Connect(string database, string server, string configFile, bool manualConnection)
        {
            ActiveConfig.OpenDatabase(server, database);

            if (configFile != null)
            {
                ActiveConfig.LoadXml(configFile);
            }
            else
            {
                ActiveConfig.XmlConfig = new XmlConfigFile();
            }

            Global.Connected = true;

            ActiveConfig.DB.CanCache = true;

            if (manualConnection)
            {
                AcmaCmdletConnected.isManuallyConnected = true;
                AcmaCmdletConnected.isAutoConnected = false;
            }
        }

        protected bool TryAutoConnect()
        {
            RegistryKey registry = Registry.LocalMachine.OpenSubKey("Software\\Lithnet\\ACMA");

            if (registry == null)
            {
                return false;
            }

            string dbname = (string)registry.GetValue("DatabaseName", string.Empty);
            string server = (string)registry.GetValue("ServerName", string.Empty);

            if (string.IsNullOrEmpty(dbname))
            {
                dbname = "Lithnet.Acma";
            }

            if (string.IsNullOrEmpty(server))
            {
                server = "localhost";
            }

            try
            {
                this.Connect(dbname, server, null, false);
                AcmaCmdletConnected.isAutoConnected = true;
                AcmaCmdletConnected.isManuallyConnected = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool PromptForContinuePermission()
        {
            if (!this.ShouldContinue("No configuration file is loaded. Are you sure you want to continue?", "Configuration file not loaded", ref yesToAll, ref noToAll))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
