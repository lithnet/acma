using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Drawing;

namespace Lithnet.Acma.TestEngine
{
    public class UnitTestOutcomes : IEnumerable<UnitTestOutcome>
    {
        public UnitTestOutcomes()
        {
            this.Outcomes = new List<UnitTestOutcome>();
        }

        private List<UnitTestOutcome> Outcomes { get;set; }

        public string TestFile { get; set; }
        public string ConfigFile { get; set; }
        public string Database { get; set;}
        public string Server { get; set; }

        public int TestCount
        {
            get
            {
                return this.Outcomes.Count;
            }
        }

        public int TestSuccessfulCount
        {
            get
            {
                return this.Outcomes.Count(t => t.Result == UnitTestResult.Passed);
            }
        }

        public int TestFailedCount
        {
            get
            {
                return this.Outcomes.Count(t => t.Result == UnitTestResult.Failed);
            }
        }

        public int TestInconclusiveCount
        {
            get
            {
                return this.Outcomes.Count(t => t.Result == UnitTestResult.Inconclusive);
            }
        }

        public int TestErrorCount
        {
            get
            {
                return this.Outcomes.Count(t => t.Result == UnitTestResult.Error);
            }
        }

        public int TestSuccessfulPercent
        {
            get
            {
                if (this.TestCount == 0)
                {
                    return 0;
                }

                return Convert.ToInt32(Convert.ToDecimal(this.TestSuccessfulCount) / this.TestCount * 100);
            }
        }

        public int TestFailedPercent
        {
            get
            {
                if (this.TestCount == 0)
                {
                    return 0;
                }

                return Convert.ToInt32(Convert.ToDecimal(this.TestFailedCount) / this.TestCount * 100);
            }
        }

        public int TestInconclusivePercent
        {
            get
            {
                if (this.TestCount == 0)
                {
                    return 0;
                }

                return Convert.ToInt32(Convert.ToDecimal(this.TestInconclusiveCount) / this.TestCount * 100);
            }
        }

        public int TestErrorPercent
        {
            get
            {
                if (this.TestCount == 0)
                {
                    return 0;
                }

                return Convert.ToInt32(Convert.ToDecimal(this.TestErrorCount) / this.TestCount * 100);
            }
        }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration
        {
            get
            {
                if (this.StartTime.Ticks == 0 || this.EndTime.Ticks == 0)
                {
                    return new TimeSpan(0);
                }

                return this.EndTime.Subtract(this.StartTime);
            }
        }

        public decimal TestsPerSecond
        {
            get
            {
                return Convert.ToDecimal(this.TestCount) / Convert.ToDecimal(this.Duration.TotalSeconds);
            }
        }

        public void Add(UnitTestOutcome outcome)
        {
            this.Outcomes.Add(outcome);
        }

        public void AddRange(IEnumerable<UnitTestOutcome> items)
        {
            this.Outcomes.AddRange(items);
        }

        public void Clear()
        {
            this.Outcomes.Clear();
        }

        public IEnumerator<UnitTestOutcome> GetEnumerator()
        {
            return this.Outcomes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Outcomes.GetEnumerator();
        }

        public void ToXml(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = Environment.NewLine;
            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("acma-unit-test-results");

            writer.WriteStartElement("statistics");
            writer.WriteElementString("test-count", this.TestCount.ToString());
            writer.WriteElementString("test-count-successful", this.TestSuccessfulCount.ToString());
            writer.WriteElementString("test-count-failed", this.TestFailedCount.ToString());
            writer.WriteElementString("test-count-inconclusive", this.TestInconclusiveCount.ToString());
            writer.WriteElementString("test-count-error", this.TestErrorCount.ToString());
            writer.WriteElementString("test-start-time", this.StartTime.ToString("g"));
            writer.WriteElementString("test-end-time", this.EndTime.ToString("g"));
            writer.WriteElementString("test-duration", this.Duration.ToString("g"));
            writer.WriteElementString("tests-per-second", this.TestsPerSecond.ToString("F2"));
            writer.WriteEndElement(); // </summary>

            writer.WriteStartElement("results");

            foreach(UnitTestOutcome outcome in this.Outcomes)
            {
                writer.WriteStartElement("result");
                writer.WriteElementString("test-id", outcome.Test.ID);
                writer.WriteElementString("test-description", outcome.Test.Description);
                writer.WriteElementString("test-result", outcome.Result.ToString());
                writer.WriteElementString("test-result-description", outcome.Description);
                writer.WriteEndElement(); // </result>
            }

            writer.WriteEndElement(); // </results>

            writer.WriteEndElement(); // </acma-unit-test-results>
            writer.WriteEndDocument();
            writer.Close();
        }

        public void ToHtml(string filename)
        {
            Report report = new Report();
            report.ReportHeading = new ReportingCell ("ACMA Unit Test Report");
            report.VersionText = System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString(3);

            if (this.TestErrorCount > 0)
            {
                report.ReportSubHeading = new ReportingCell("Errors were encountered during the test run", this.GetResultColor(UnitTestResult.Error));
            }
            else if (this.TestFailedCount > 0)
            {
                report.ReportSubHeading = new ReportingCell("One or more unit tests failed", this.GetResultColor(UnitTestResult.Failed));
            }
            else if (this.TestInconclusiveCount > 0)
            {
                report.ReportSubHeading = new ReportingCell("One or more unit test results were inconclusive", this.GetResultColor(UnitTestResult.Inconclusive));
            }
            else if (this.TestSuccessfulCount == 0)
            {
                report.ReportSubHeading = new ReportingCell("No unit tests were executed", Color.Black);
            }
            else
            {
                report.ReportSubHeading = new ReportingCell("The unit test run was successful", this.GetResultColor(UnitTestResult.Passed));
            }

            ReportingItemSection parameters = new ReportingItemSection();
            parameters.SectionTitle = "Test Parameters";
            parameters.AddReportingItem("Test file", this.TestFile);
            parameters.AddReportingItem("ACMA configuration file", this.ConfigFile);
            parameters.AddReportingItem("SQL server", this.Server);
            parameters.AddReportingItem("Database", this.Database);

            report.Sections.Add(parameters);

            ReportingItemSection stats = new ReportingItemSection();
            stats.SectionTitle = "Test Statistics";
            stats.AddReportingItem("Test count", this.TestCount.ToString());
            stats.AddReportingItem("Successful tests", string.Format("{0} ({1}%)", this.TestSuccessfulCount.ToString(), this.TestSuccessfulPercent.ToString("D")));
            stats.AddReportingItem("Failed tests",  string.Format("{0} ({1}%)", this.TestFailedCount.ToString(), this.TestFailedPercent.ToString("D")));
            stats.AddReportingItem("Inconclusive tests",  string.Format("{0} ({1}%)", this.TestInconclusiveCount.ToString(), this.TestInconclusivePercent.ToString("D")));
            stats.AddReportingItem("Errored tests",  string.Format("{0} ({1}%)", this.TestErrorCount.ToString(), this.TestErrorPercent.ToString("D")));
            stats.AddReportingItem("Test start time", this.StartTime.ToString("g"));
            stats.AddReportingItem("Test end time", this.EndTime.ToString("g"));
            stats.AddReportingItem("Test duration", this.Duration.ToString("g"));
            stats.AddReportingItem("Test per second", this.TestsPerSecond.ToString("F2"));

            report.AddSection(stats);

            ReportingItemSection results = new ReportingItemSection();
            results.SectionTitle = "Test Results";
            results.AddHeading("Result", Color.Black, Color.LightGray);
            results.AddHeading("Test ID", Color.Black, Color.LightGray);
            results.AddHeading("Test description", Color.Black, Color.LightGray);
            results.AddHeading("Result description", Color.Black, Color.LightGray);
            

            foreach (UnitTestOutcome outcome in this.Outcomes)
            {
                ReportingItem item = new ReportingItem();
                item.Title = outcome.Result.ToString();
                item.TitleBackgroundColor = this.GetResultColor(outcome.Result);
                item.TitleTextColor = Color.Black;
                item.AddValue(outcome.Test.ID);
                item.AddValue(outcome.Test.Description);
                item.AddValue(outcome.Description);
                results.ReportingItems.Add(item);
            }

            report.AddSection(results);

            string content = report.GenerateHtmlReport();
            File.WriteAllText(filename, content);

        }

        private Color GetResultColor(UnitTestResult result)
        {
            switch (result)
            {
                case UnitTestResult.Passed:
                    return ColorTranslator.FromHtml("#8DBB70");
                
                case UnitTestResult.Failed:
                    return ColorTranslator.FromHtml("#F0BB44");

                case UnitTestResult.Error:
                    return ColorTranslator.FromHtml("#F24F4F");

                case UnitTestResult.Inconclusive:
                    return ColorTranslator.FromHtml("#F8943F");
                
                default:
                    throw new ArgumentException("Unknown value");
            }
        }
    
    }
}
