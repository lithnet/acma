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
    [Cmdlet(VerbsData.Export, "AcmaObjects")]
    public class ExportAcmaObjectsCmdLet : Cmdlet
    {
        [Parameter(Mandatory = true,
            HelpMessage = "The name of the XML file to export",
            HelpMessageBaseName = "ExportFile",
            Position = 0,
            ValueFromPipeline = false)]
        public string FileName { get; set; }

        [Parameter(Mandatory = true,
           HelpMessage = "The objects to export",
           HelpMessageBaseName = "ExportObjects",
           Position = 1,
           ValueFromPipeline = true)]

        public AcmaPSObject[] ExportObjects { get; set; }

        private DateTime operationStartTime;

        private DateTime sampleIntervalStartTime;

        private int totalObjectCount;

        private int currentObjectCount;

        private int sampleIntervalStartCount;

        private int sampleOpsSec;

        private int averageOpsSec;

        private int secondsRemaining;

        private Timer timer;

        private XmlWriter writer;

        private ProgressRecord progress;

        protected override void BeginProcessing()
        {
            try
            {
                if (this.ExportObjects == null || this.ExportObjects.Length == 0)
                {
                    ThrowTerminatingError(new ErrorRecord(new ArgumentNullException("ExportObjects"), "NoObjects", ErrorCategory.InvalidArgument, null));
                }

                this.totalObjectCount = this.ExportObjects.Length;

                Global.ThrowIfNotConnected(this);
                this.writer = null;
                MAStatistics.StartOperation(MAOperationType.Import);
                Console.WriteLine("Writing export file... ");

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";
                settings.NewLineChars = Environment.NewLine;
                writer = XmlWriter.Create(this.FileName, settings);
                writer.WriteStartDocument();
                writer.WriteStartElement("acma-export");
                progress = new ProgressRecord(0, "Export", "Starting export");
                this.operationStartTime = DateTime.Now;
                this.sampleIntervalStartTime = DateTime.Now;

                this.timer = new Timer(1000);
                this.timer.Elapsed += this.timer_Elapsed;
                this.timer.Start();

                progress.RecordType = ProgressRecordType.Processing;
                progress.PercentComplete = 0;
                this.WriteProgress(progress);
            }
            catch (Exception ex)
            {
                if (this.timer != null)
                {
                    this.timer.Stop();
                    this.timer.Elapsed -= this.timer_Elapsed;
                    this.timer = null;
                }

                if (writer != null)
                {
                    writer.Close();
                }

                ThrowTerminatingError(new ErrorRecord(ex, "UnknownError", ErrorCategory.InvalidOperation, null));

            }
        }

        protected override void EndProcessing()
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();

            progress.RecordType = ProgressRecordType.Completed;
            WriteProgress(progress);

            Console.WriteLine("Done");
            MAStatistics.StopOperation();
            Console.WriteLine(MAStatistics.ToString());

            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Elapsed -= this.timer_Elapsed;
            }

            if (writer != null)
            {
                writer.Close();
            }
        }

        protected override void StopProcessing()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Elapsed -= this.timer_Elapsed;
            }

            if (writer != null)
            {
                writer.Close();
            }
        }

        protected override void ProcessRecord()
        {
            foreach (AcmaPSObject obj in this.ExportObjects)
            {
                try
                {
                    obj.InternalHologram.PreLoadAVPs();
                    CSEntryChangeXmlExport.ExportToXml(obj.InternalHologram, writer);
                    MAStatistics.AddImportOperation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error exporting object {0}: {1}: {2}", this.currentObjectCount, obj.ObjectID, ex.Message);
                }

                //progress.PercentComplete = Convert.ToInt32(((decimal)this.currentObjectCount / this.totalObjectCount) * 100);
                progress.StatusDescription = string.Format("Importing {0}/{1}... {2} ({3} objects/sec)", this.currentObjectCount, this.totalObjectCount, obj.ObjectID, this.sampleOpsSec);
                progress.CurrentOperation = string.Format("Average rate: {0} objects/sec", this.averageOpsSec);
                progress.SecondsRemaining = this.secondsRemaining;

                this.WriteProgress(progress);

                this.currentObjectCount++;

                obj.Dispose();
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
