using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lithnet.Acma;
using Lithnet.Fim.Core;
using Lithnet.Acma.TestEngine;
using System.IO;
using Lithnet.Logging;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsLifecycle.Invoke, "AcmaUnitTests")]
    public class InvokeAcmaUnitTests : Cmdlet
    {
        private int indentLevel = 0;

        [Parameter(Mandatory = false,
            HelpMessage = "The name of the file to save the HTML report to",
            HelpMessageBaseName = "Report file name",
            Position = 1,
            ValueFromPipeline = false)]
        public string HtmlReportFileName { get; set; }

        [Parameter(Mandatory = false,
            HelpMessage = "Stops executing unit tests when a failure is encountered",
            HelpMessageBaseName = "Break on test failure",
            Position = 2,
            ValueFromPipeline = false)]
        public bool BreakOnTestFailure { get; set; }

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                UnitTestFile file = this.LoadUnitTestFile();
                UnitTestFile.BreakOnTestFailure = this.BreakOnTestFailure;

                MAStatistics.StartOperation(MAOperationType.Export);
                Console.WriteLine(string.Empty.PadRight(79, '-'));
                Console.WriteLine("Executing unit tests from: {0}", Path.GetFileName(ActiveConfig.XmlConfig.FileName));
                Console.WriteLine();
                UnitTest.UnitTestStartingEvent += UnitTest_UnitTestStartingEvent;
                UnitTest.UnitTestCompletedEvent += UnitTest_UnitTestCompletedEvent;
                UnitTestGroup.UnitTestGroupStartingEvent += UnitTestGroup_UnitTestGroupStartingEvent;
                UnitTestGroup.UnitTestGroupCompletedEvent += UnitTestGroup_UnitTestGroupCompletedEvent;
                UnitTestOutcomes report = file.Execute(Global.DataContext);
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

                if (!string.IsNullOrWhiteSpace(this.HtmlReportFileName))
                {
                    Console.Write("Writing test results to: {0}... ", this.HtmlReportFileName);
                    report.ToHtml(this.HtmlReportFileName);
                    Console.WriteLine("Done");
                }
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, null);
                ThrowTerminatingError(error);
            }
            finally
            {
                UnitTest.UnitTestStartingEvent -= UnitTest_UnitTestStartingEvent;
                UnitTest.UnitTestCompletedEvent -= UnitTest_UnitTestCompletedEvent;
                UnitTestGroup.UnitTestGroupStartingEvent -= UnitTestGroup_UnitTestGroupStartingEvent;
                UnitTestGroup.UnitTestGroupCompletedEvent -= UnitTestGroup_UnitTestGroupCompletedEvent;
            }
        }

        private UnitTestFile LoadUnitTestFile()
        {
            return UnitTestFile.LoadXml(ActiveConfig.XmlConfig.FileName);
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
