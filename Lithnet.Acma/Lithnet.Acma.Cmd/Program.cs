using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.Logging;
using System.IO;
using Lithnet.Acma.TestEngine;
using Lithnet.Acma;
using Lithnet.Fim.Transforms;
using System.Xml.Serialization;
using System.Xml;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Cmd
{
    public class Program
    {
        private const int ExitCodeInvalidArguments = 87;

        public static void Main(string[] args)
        {
            ShowBanner();

            if (CommandLineHelper.IsHelpCommandLineArgumentPresent())
            {
                ShowHelp();
                return;
            }

            try
            {
                LaunchAction();
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                Console.WriteLine(Logger.GetExceptionText(ex, false));
            }
            finally
            {
                Logger.CloseLog();
                Console.ResetColor();
            }
        }

        private static void ShowBanner()
        {
            Console.WriteLine("acmacmd.exe v{0} - ACMA command line tool", GetAssemblyVersion(3));
            Console.WriteLine("");
        }

        private static void LaunchAction()
        {
            if (CommandLineHelper.IsCommandLineArgumentPresent("unittest"))
            {
                LaunchActionUnitTest();
                return;
            }
            else
            {
                Console.WriteLine("No recognized command line arguments were present");
                ShowHelp();
            }
        }

        private static void LaunchActionUnitTest()
        {
            CheckForMissingArguments("s", "d", "c");

            string server = CommandLineHelper.GetCommandLineArgumentValue("s");
            string database = CommandLineHelper.GetCommandLineArgumentValue("d");
            string configFile = CommandLineHelper.GetCommandLineArgumentValue("c");

            string xmlOutFile = CommandLineHelper.GetCommandLineArgumentValue("x");
            string htmlOutFile = CommandLineHelper.GetCommandLineArgumentValue("h");

            ActiveConfig.OpenDatabase(server, database);
            MADataContext dc = ActiveConfig.DB.MADataConext;

            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            ActiveConfig.LoadXml(configFile);
            UINotifyPropertyChanges.EndIgnoreAllChanges();

            UnitTestExecutor executor = new UnitTestExecutor();
            executor.LaunchUnitTests(configFile, xmlOutFile, htmlOutFile, dc);
        }
        
        public static string GetAssemblyVersion(int fieldCount = 4)
        {
            return System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString(fieldCount);
        }
     
        private static MADataContext CreateMADataContext(string server, string database)
        {
            return new MADataContext(server, database);
        }

        private static void CheckForMissingArguments(params string[] args)
        {
            foreach (string arg in args)
            {
                if (!CommandLineHelper.IsCommandLineArgumentPresent(arg))
                {
                    Console.WriteLine(string.Format("Missing command line argument /{0}", arg));
                    ShowHelp(ExitCodeInvalidArguments);
                }
            }
        }

        private static void ShowHelp(int exitCode = 0)
        {
            //                 --------------------------------------------------------------------------------
            Console.WriteLine(string.Empty.PadLeft(80, '-'));
            Console.WriteLine("Performs actions against the ACMA database");
            Console.WriteLine();
            Console.WriteLine("acmacmd.exe     /unittest /f:unittestfilename /s:server /d:database /c:config");
            Console.WriteLine("                [/log:logfile] [/x:xmloutput] [/h:htmloutput]");
            Console.WriteLine();
            Console.WriteLine("acmacmd.exe     /export /s:server /d:database /c:config /x:xmloutput");
            Console.WriteLine();
            Console.WriteLine(" [no args]            Shows the help screen");
            Console.WriteLine(" /unittest            Performs unit tests using the specified test file");
            Console.WriteLine(" /exports             Exports all objects from the ACMA database to the");
            Console.WriteLine("                      specified XML file");
            Console.WriteLine(" /f:unittestfilename  The file containing the unit tests to perform");
            Console.WriteLine(" /s:server            The name of the SQL server hosting the ACMA database");
            Console.WriteLine(" /d:database          The name of the database");
            Console.WriteLine(" /c:config            The ACMA configuration file");
            Console.WriteLine(" /log:logfile         The name of the log file");
            Console.WriteLine(" /x:xmloutput         Exports the results to the specified XML file");
            Console.WriteLine(" /h:htmloutput        Exports the unit test results to the specified HTML file");
            Console.WriteLine();
            Console.WriteLine();

            Environment.Exit(exitCode);
        }
    }
}
