using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Host;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Lithnet.Logging;
using System.Timers;

namespace Lithnet.Acma.PS
{
    [Cmdlet(VerbsData.Import, "AcmaObjects")]
    public class ImportAcmaObjectsCmdLet : Cmdlet
    {
        [Parameter(Mandatory = true,
            HelpMessage = "The name of the XML file to import",
            HelpMessageBaseName = "ImportFile",
            Position = 0,
            ValueFromPipeline = true)]
        public string FileName { get; set; }

        [Parameter(Mandatory = false,
            HelpMessage = "Specifies if attributes in the XML file that are not found in the schema should be ignored",
            HelpMessageBaseName = "ImportFile",
            Position = 1, 
            ValueFromPipeline = true)]
        public SwitchParameter IgnoreMissingAttributes { get; set; }

        private DateTime operationStartTime;

        private DateTime sampleIntervalStartTime;

        private int totalObjectCount;

        private int currentObjectCount;

        private int sampleIntervalStartCount;

        private int sampleOpsSec;

        private int averageOpsSec;

        private int secondsRemaining;

        private Timer timer;

        protected override void ProcessRecord()
        {
            Global.ThrowIfNotConnected(this);

            try
            {
                MAStatistics.StartOperation(MAOperationType.Export);
                ActiveConfig.DB.CanCache = true;

                Console.WriteLine("Reading import file... ");

                StreamReader r = new StreamReader(this.FileName);
                XmlReader reader = XmlReader.Create(r);
                var doc = XDocument.Load(reader);

                Console.WriteLine("Importing objects... ");
                this.totalObjectCount = doc.Root.Elements().Count();
                ProgressRecord progress = new ProgressRecord(0, "Import", "Starting import");

                this.operationStartTime = DateTime.Now;
                this.sampleIntervalStartTime = DateTime.Now;

                this.timer = new Timer(1000);
                this.timer.Elapsed += this.timer_Elapsed;
                this.timer.Start();

                progress.RecordType = ProgressRecordType.Processing;
                progress.PercentComplete = 0;
                this.WriteProgress(progress);

                foreach (var node in doc.Root.Elements("object-change"))
                {
                    bool refretry;
                    //XElement element = node;
                    AcmaCSEntryChange csentry = CSEntryChangeXmlImport.ImportFromXml(node, !this.IgnoreMissingAttributes.IsPresent);

                    progress.PercentComplete = Convert.ToInt32(((decimal)this.currentObjectCount / this.totalObjectCount) * 100);
                    progress.StatusDescription = string.Format("Importing {0}/{1}... {2} ({3} objects/sec)", this.currentObjectCount, this.totalObjectCount, csentry.DN, this.sampleOpsSec);
                    progress.CurrentOperation = string.Format("Average rate: {0} objects/sec", this.averageOpsSec);
                    progress.SecondsRemaining = this.secondsRemaining;

                    this.WriteProgress(progress);

                    try
                    {
                        CSEntryExport.PutExportEntry(csentry, out refretry);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error importing object {0}: {1}: {2}", this.currentObjectCount, csentry.DN, ex.Message);
                    }

                    this.currentObjectCount++;
                }

                progress.RecordType = ProgressRecordType.Completed;
                WriteProgress(progress);

                Console.WriteLine("Done");
                MAStatistics.StopOperation();
                Console.WriteLine(MAStatistics.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                ErrorRecord error = new ErrorRecord(ex, "UnknownError", ErrorCategory.NotSpecified, this.FileName);
                ThrowTerminatingError(error);
            }
            finally
            {
                if (this.timer != null)
                {
                    this.timer.Stop();
                    this.timer.Elapsed -= this.timer_Elapsed;
                }
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan currentDuration = DateTime.Now.Subtract(this.operationStartTime);
            TimeSpan sampleIntervalDuration = DateTime.Now.Subtract(this.sampleIntervalStartTime);

            if (currentDuration.TotalSeconds > 0)
            {
                this.averageOpsSec = (int)(this.currentObjectCount / currentDuration.TotalSeconds);
            }

            if (this.averageOpsSec > 0)
            {
                this.secondsRemaining = (int)((this.totalObjectCount - this.currentObjectCount) / this.averageOpsSec);
            }

            int sampleObjectCount = this.currentObjectCount - this.sampleIntervalStartCount;

            if (sampleIntervalDuration.TotalSeconds > 0)
            {
                this.sampleOpsSec = (int)(sampleObjectCount / sampleIntervalDuration.TotalSeconds);
            }

            this.sampleIntervalStartTime = DateTime.Now;
            this.sampleIntervalStartCount = this.currentObjectCount;
        }
    }
}
