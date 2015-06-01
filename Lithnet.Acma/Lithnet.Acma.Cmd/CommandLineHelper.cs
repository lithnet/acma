using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.Acma.Cmd
{
    public static class CommandLineHelper
    {
        public static string GetCommandLineArgumentValue(string argumentName)
        {
            argumentName = CleanArgument(argumentName);

            if (!argumentName.EndsWith(":"))
            {
                argumentName = argumentName + ":";
            }

            string value = Environment.GetCommandLineArgs().FirstOrDefault(t =>
                t.StartsWith("/" + argumentName, StringComparison.OrdinalIgnoreCase) ||
                t.StartsWith("-" + argumentName, StringComparison.OrdinalIgnoreCase));

            if (value != null)
            {
                value = value.Remove(0, argumentName.Length + 1);
                value = value.Trim('\"');
            }

            return value;
        }

        private static string CleanArgument(string argumentName)
        {
            if (argumentName.StartsWith("/") || argumentName.StartsWith("-"))
            {
                argumentName = argumentName.Remove(0, 1);
            }

            return argumentName;
        }

        public static bool IsCommandLineArgumentPresent(string argumentName)
        {
            argumentName = CleanArgument(argumentName);

            return Environment.GetCommandLineArgs().Any(t =>
                t.Equals("/" + argumentName, StringComparison.OrdinalIgnoreCase) ||
                t.Equals("-" + argumentName, StringComparison.OrdinalIgnoreCase) ||
                t.StartsWith("/" + argumentName + ":", StringComparison.OrdinalIgnoreCase) ||
                t.StartsWith("-" + argumentName + ":", StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsHelpCommandLineArgumentPresent()
        {
            return Environment.GetCommandLineArgs().Any(t =>
                t.Equals("/?", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("-?", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("/help", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("-help", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("/h", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("-h", StringComparison.OrdinalIgnoreCase));
        }
    }
}
