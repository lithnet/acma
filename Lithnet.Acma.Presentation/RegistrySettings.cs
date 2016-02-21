using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Lithnet.Common;
using Lithnet.Acma;
using Lithnet.Common.ObjectModel;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.Presentation
{
    public static class RegistrySettings
    {
        private static RegistryKey key;

        private static RegistryKey RootKey
        {
            get
            {
                if (RegistrySettings.key == null)
                {
                    RegistrySettings.key = Registry.CurrentUser.CreateSubKey(@"Software\Lithnet\Acma\Editor");
                }

                return RegistrySettings.key;
            }
        }

        public static void SetValue(string name, object value)
        {
            if (value is string)
            {
                RegistrySettings.RootKey.SetValue(name, value, RegistryValueKind.String);
            }
            else if (value is int)
            {
                RegistrySettings.RootKey.SetValue(name, value, RegistryValueKind.DWord);
            }
            else if (value is long)
            {
                RegistrySettings.RootKey.SetValue(name, value, RegistryValueKind.QWord);
            }
            else if (value is byte[])
            {
                RegistrySettings.RootKey.SetValue(name, value, RegistryValueKind.Binary);
            }
            else if (value is IEnumerable<string>)
            {
                string[] values = ((IEnumerable<string>)value).ToArray();
                if (values != null)
                {
                    RegistrySettings.RootKey.SetValue(name, values, RegistryValueKind.MultiString);
                }
            }
        }

        public static T GetValue<T>(string name, T defaultValue)
        {
            return (T)RegistrySettings.RootKey.GetValue(name, defaultValue);
        }

        public static object GetValue(string name, object defaultValue)
        {
            return RegistrySettings.RootKey.GetValue(name, defaultValue);
        }
    }
}
