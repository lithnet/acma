using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Lithnet.Acma.TestEngine;
using Lithnet.Acma;
using Lithnet.Common.ObjectModel;
using Lithnet.Fim.Core;

namespace Lithnet.Acma.Cmd
{
    public class UnitTestExecutor
    {
        private int indentLevel = 0;

        public void LaunchUnitTests(string testFileName, string xmlOutFileName, string htmlOutputFileName, MADataContext dc)
        {
            if (!File.Exists(testFileName))
            {
                throw new FileNotFoundException(testFileName);
            }

            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            UnitTestFile tests = UnitTestFile.LoadXml(testFileName);
            UINotifyPropertyChanges.EndIgnoreAllChanges();

            if (CommandLineHelper.IsCommandLineArgumentPresent("break"))
            {
                UnitTestFile.BreakOnTestFailure = true;
            }

            MAStatistics.StartOperation(MAOperationType.Export);
            Console.WriteLine(string.Empty.PadRight(79, '-'));
            Console.WriteLine("Executing unit tests from: {0}", Path.GetFileName(testFileName));
            Console.WriteLine();
            UnitTest.UnitTestStartingEvent += UnitTest_UnitTestStartingEvent;
            UnitTest.UnitTestCompletedEvent += UnitTest_UnitTestCompletedEvent;
            UnitTestGroup.UnitTestGroupStartingEvent += UnitTestGroup_UnitTestGroupStartingEvent;
            UnitTestGroup.UnitTestGroupCompletedEvent += UnitTestGroup_UnitTestGroupCompletedEvent;
            UnitTestOutcomes report = tests.Execute(dc);
            MAStatistics.StopOperation();
            Console.WriteLine();

            Console.WriteLine(string.Empty.PadRight(79, '-'));
            if (report.TestCount > 0)
            {
                Console.WriteLine("Executed {0} tests", report.TestCount);

                if (report.TestCount == report.TestSuccessfulCount)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine("{0} ({1}%) successful", report.TestSuccessfulCount, report.TestSuccessfulPercent.ToString("D"));
                Console.ForegroundColor = ConsoleColor.Gray;

                if (report.TestFailedCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine("{0} ({1}%) failed", report.TestFailedCount, report.TestFailedPercent.ToString("D"));
                Console.ForegroundColor = ConsoleColor.Gray;

                if (report.TestInconclusiveCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine("{0} ({1}%) inconclusive", report.TestInconclusiveCount, report.TestInconclusivePercent.ToString("D"));
                Console.ForegroundColor = ConsoleColor.Gray;


                if (report.TestErrorCount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine("{0} ({1}%) error{2}", report.TestErrorCount, report.TestErrorPercent.ToString("D"), report.TestErrorCount == 1 ? string.Empty : "s");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.WriteLine("No tests were executed");
            }
            Console.WriteLine(string.Empty.PadRight(79, '-'));

            Console.WriteLine(MAStatistics.ToString());

            Console.WriteLine(string.Empty.PadRight(79, '-'));

            if (!string.IsNullOrWhiteSpace(xmlOutFileName))
            {
                Console.Write("Writing test results to: {0}... ", xmlOutFileName);
                report.ToXml(xmlOutFileName);
                Console.WriteLine("Done");
            }

            if (!string.IsNullOrWhiteSpace(htmlOutputFileName))
            {
                Console.Write("Writing test results to: {0}... ", htmlOutputFileName);
                report.ToHtml(htmlOutputFileName);
                Console.WriteLine("Done");
            }

        }

        private void UnitTestGroup_UnitTestGroupCompletedEvent(object sender, IList<UnitTestOutcome> outcome)
        {
            if (this.indentLevel > 0)
            {
                this.indentLevel--;
            }
        }

        private void UnitTestGroup_UnitTestGroupStartingEvent(object sender)
        {
            UnitTestGroup test = sender as UnitTestGroup;
            string indent = string.Empty.PadRight(this.indentLevel * 3, ' ');
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(string.Format("{0}{1}", indent, test.ID).PadRight(65));
            Console.ForegroundColor = ConsoleColor.Gray;
            this.indentLevel++;
        }

        private void UnitTest_UnitTestCompletedEvent(object sender, UnitTestOutcome outcome)
        {
            UnitTest test = sender as UnitTest;
            string result = string.Empty;
            string additionaldata = string.Empty;

            switch (outcome.Result)
            {
                case UnitTestResult.Passed:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case UnitTestResult.Inconclusive:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case UnitTestResult.Failed:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case UnitTestResult.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(outcome.Description))
            {
                Console.WriteLine("{0} ", outcome.Result.ToString());
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine(outcome.Description);
            }
            else
            {
                Console.WriteLine("{0}", outcome.Result.ToString());
            }

            Console.ResetColor();
        }

        private void UnitTest_UnitTestStartingEvent(object sender)
        {
            UnitTest test = sender as UnitTest;
            string indent = string.Empty.PadRight(this.indentLevel * 3, ' ');
            Console.Write(string.Format("{0}{1}... ", indent, test.ID).TruncateString(65).PadRight(65));
        }
    }
}
