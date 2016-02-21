// -----------------------------------------------------------------------
// <copyright file="MAStatistics.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Tracks statistics related to the MA operation
    /// </summary>
    public static class MAStatistics
    {
        private static int transactionExportAddCount = 0;
        private static int transactionExportDeleteCount = 0;
        private static int transactionExportReplaceCount = 0;
        private static int transactionExportUpdateCount = 0;
        private static int transactionImportCount = 0;
        private static int transactionShadowAddCount = 0;
        private static int transactionShadowDeleteCount = 0;
        private static int transactionInheritedUpdateCount = 0;
        private static int transactionBackLinkUpdateCount = 0;

        /// <summary>
        /// Gets the total number of objects exported
        /// </summary>
        public static int ExportCount
        {
            get
            {
                return MAStatistics.ExportAddCount +
                    MAStatistics.ExportDeleteCount +
                    MAStatistics.ExportReplaceCount +
                    MAStatistics.ExportUpdateCount;
            }
        }

        public static int OperationCount
        {
            get
            {
                return MAStatistics.ExportCount +
                    MAStatistics.ShadowDeleteCount +
                    MAStatistics.ShadowAddCount +
                    MAStatistics.BackLinkUpdateCount +
                    MAStatistics.InheritedUpdateCount;
            }
        }

        /// <summary>
        /// Gets the type of the current operation
        /// </summary>
        public static MAOperationType CurrentOperation { get; private set; }

        /// <summary>
        /// Gets the number of objects added
        /// </summary>
        public static int ExportAddCount { get; private set; }

        public static int ShadowAddCount { get; private set; }

        public static int RolledBackOperationCount { get; private set; }

        /// <summary>
        /// Gets the number of objects updated
        /// </summary>
        public static int ExportUpdateCount { get; private set; }

        public static int InheritedUpdateCount { get; private set; }

        public static int BackLinkUpdateCount { get; private set; }

        /// <summary>
        /// Gets the number of objects deleted
        /// </summary>
        public static int ExportDeleteCount { get; private set; }

        public static int ShadowDeleteCount { get; private set; }

        /// <summary>
        /// Gets the number of objects replaced
        /// </summary>
        public static int ExportReplaceCount { get; private set; }

        /// <summary>
        /// Gets the number of objects with export errors
        /// </summary>
        public static int ExportErrors { get; private set; }

        /// <summary>
        /// Gets the number of reference retry requests made to the sync engine
        /// </summary>
        public static int ReferenceRetryRequests { get; private set; }

        /// <summary>
        /// Gets the total number of objects imported
        /// </summary>
        public static int ImportCount { get; private set; }

        /// <summary>
        /// Gets the number of import objects with errors
        /// </summary>
        public static int ImportErrors { get; private set; }

        /// <summary>
        /// Gets the average number of imported objects per second
        /// </summary>
        public static double ImportsPerSecond
        {
            get
            {
                if (MAStatistics.OperationDuration.Ticks == 0 || MAStatistics.ImportCount == 0)
                {
                    return 0;
                }

                return MAStatistics.ImportCount / MAStatistics.OperationDuration.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets the average number of exported objects per second
        /// </summary>
        public static double ExportsPerSecond
        {
            get
            {
                if (MAStatistics.OperationDuration.Ticks == 0 || MAStatistics.ExportCount == 0)
                {
                    return 0;
                }

                return MAStatistics.ExportCount / MAStatistics.OperationDuration.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets the average number of exported objects per second
        /// </summary>
        public static double ObjectOperationsPerSecond
        {
            get
            {
                if (MAStatistics.OperationDuration.Ticks == 0 || MAStatistics.ExportCount == 0)
                {
                    return 0;
                }

                return (MAStatistics.OperationCount | MAStatistics.RolledBackOperationCount) / MAStatistics.OperationDuration.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets the time that the current or most recent operation started
        /// </summary>
        public static DateTime OperationStartTime { get; private set; }

        /// <summary>
        /// Gets the time that the current or most recent operation ended
        /// </summary>
        public static DateTime OperationEndTime { get; private set; }

        /// <summary>
        /// Gets the total duration of the most recent operation
        /// </summary>
        public static TimeSpan OperationDuration
        {
            get
            {
                if (OperationStartTime.Ticks == 0 || OperationEndTime.Ticks == 0)
                {
                    return new TimeSpan(0);
                }
                else
                {
                    return OperationEndTime.Subtract(OperationStartTime);
                }
            }
        }

        /// <summary>
        /// Resets the statistics
        /// </summary>
        public static void Reset()
        {
            CurrentOperation = MAOperationType.None;
            OperationStartTime = new DateTime();
            OperationEndTime = new DateTime();
            ExportAddCount = 0;
            ExportDeleteCount = 0;
            ExportErrors = 0;
            ExportReplaceCount = 0;
            ExportUpdateCount = 0;
            ImportCount = 0;
            ImportErrors = 0;
            ShadowAddCount = 0;
            ShadowDeleteCount = 0;
            InheritedUpdateCount = 0;
            BackLinkUpdateCount = 0;
            RolledBackOperationCount = 0;
        }

        /// <summary>
        /// Adds an export operation event
        /// </summary>
        /// <param name="type">The type of export operation to add</param>
        public static void AddExportOperation(ObjectModificationType type)
        {
            switch (type)
            {
                case ObjectModificationType.Add:
                    MAStatistics.ExportAddCount++;
                    break;

                case ObjectModificationType.Delete:
                    MAStatistics.ExportDeleteCount++;
                    break;
                
                case ObjectModificationType.Replace:
                    MAStatistics.ExportReplaceCount++;
                    break;
                
                case ObjectModificationType.Update:
                    MAStatistics.ExportUpdateCount++;
                    break;
                
                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds an import operation event
        /// </summary>
        public static void AddImportOperation()
        {
            MAStatistics.ImportCount++;
        }

        /// <summary>
        /// Adds an export error event
        /// </summary>
        public static void AddExportError()
        {
            MAStatistics.ExportErrors++;
        }

        /// <summary>
        /// Adds an import error event
        /// </summary>
        public static void AddImportError()
        {
            MAStatistics.ImportErrors++;
        }

        public static void AddShadowAdd()
        {
            MAStatistics.ShadowAddCount++;
        }

        public static void AddShadowDelete()
        {
            MAStatistics.ShadowDeleteCount++;
        }

        public static void AddInheritedUpdate()
        {
            MAStatistics.InheritedUpdateCount++;
        }

        public static void AddBacklinkUpdate()
        {
            MAStatistics.BackLinkUpdateCount++;
        }

        /// <summary>
        /// Adds a reference retry request event
        /// </summary>
        public static void AddReferenceRetryRequest()
        {
            MAStatistics.ReferenceRetryRequests++;
        }

        /// <summary>
        /// Resets the counters and starts a new operation
        /// </summary>
        /// <param name="operationType">The type of operation in progress</param>
        public static void StartOperation(MAOperationType operationType)
        {
            MAStatistics.Reset();
            MAStatistics.CurrentOperation = operationType;
            MAStatistics.OperationStartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the current operation
        /// </summary>
        public static void StopOperation()
        {
            MAStatistics.OperationEndTime = DateTime.Now;
        }

        /// <summary>
        /// Gets a string representation of the statistics
        /// </summary>
        /// <returns>A string representation of the statistics</returns>
        public static new string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine();
            builder.AppendLine("MA Operation Statistics");
            builder.AppendLine();
            builder.AppendLine("Operation type: " + MAStatistics.CurrentOperation.ToString());
            builder.AppendLine("Operation start time: " + MAStatistics.OperationStartTime.ToShortDateString() + " " + MAStatistics.OperationStartTime.ToShortTimeString());
            builder.AppendLine("Operation end time: " + MAStatistics.OperationEndTime.ToShortDateString() + " " + MAStatistics.OperationEndTime.ToShortTimeString());
            builder.AppendLine("Operation duration: " + MAStatistics.OperationDuration.ToString("g"));
           
            if (MAStatistics.CurrentOperation == MAOperationType.Export)
            {
                builder.AppendLine();
                builder.AppendLine("Export add: " + MAStatistics.ExportAddCount);
                builder.AppendLine("Export update: " + MAStatistics.ExportUpdateCount);
                builder.AppendLine("Export replace: " + MAStatistics.ExportReplaceCount);
                builder.AppendLine("Export delete: " + MAStatistics.ExportDeleteCount);
                builder.AppendLine("Export errors: " + MAStatistics.ExportErrors);
                builder.AppendLine("Export total: " + MAStatistics.ExportCount);
                builder.AppendLine("Exports/sec: " + MAStatistics.ExportsPerSecond.ToString("N"));
                builder.AppendLine("Reference retry requests: " + MAStatistics.ReferenceRetryRequests);

                builder.AppendLine("Shadow add: " + MAStatistics.ShadowAddCount);
                builder.AppendLine("Shadow delete: " + MAStatistics.ShadowDeleteCount);
                builder.AppendLine("Inherited update: " + MAStatistics.InheritedUpdateCount);
                builder.AppendLine("Back-link update: " + MAStatistics.BackLinkUpdateCount);
                builder.AppendLine("Operation count: " + MAStatistics.OperationCount);

                builder.AppendLine("Rolled-back operation count: " + MAStatistics.RolledBackOperationCount);

                builder.AppendLine("Ops/sec: " + MAStatistics.ObjectOperationsPerSecond.ToString("N"));
            }

            if (MAStatistics.CurrentOperation == MAOperationType.Import)
            {
                builder.AppendLine();
                builder.AppendLine("Import errors: " + MAStatistics.ExportErrors);
                builder.AppendLine("Import total: " + MAStatistics.ImportCount);
                builder.AppendLine("Imports/sec: " + MAStatistics.ImportsPerSecond.ToString("N"));
            }

            return builder.ToString();
        }

        public static void StartTransaction()
        {
            transactionExportAddCount = ExportAddCount;
            transactionExportDeleteCount = ExportDeleteCount;
            transactionExportReplaceCount = ExportReplaceCount;
            transactionExportUpdateCount = ExportUpdateCount;
            transactionImportCount = ImportCount;
            transactionShadowAddCount = ShadowAddCount;
            transactionShadowDeleteCount = ShadowDeleteCount;
            transactionInheritedUpdateCount = InheritedUpdateCount;
            transactionBackLinkUpdateCount = BackLinkUpdateCount;
        }

        public static void CompleteTransaction()
        {
        }

        public static void RollBackTransaction()
        {
            int undoExportAdds = ExportAddCount - transactionExportAddCount;
            int undoExportDelete = ExportDeleteCount - transactionExportDeleteCount;
            int undoExportReplace = ExportReplaceCount - transactionExportReplaceCount;
            int undoExportUpdate = ExportUpdateCount - transactionExportUpdateCount;
            int undoImportCount = ImportCount - transactionImportCount;
            int undoShadowAdd = ShadowAddCount - transactionShadowAddCount;
            int undoShadowDelete = ShadowDeleteCount - transactionShadowDeleteCount;
            int undoInheritedUpdateCount = InheritedUpdateCount - transactionInheritedUpdateCount;
            int undoBackLinkUpdateCount = BackLinkUpdateCount - transactionBackLinkUpdateCount;

            RolledBackOperationCount = undoExportAdds + undoExportDelete + undoExportReplace +
                undoExportUpdate + undoImportCount + undoShadowAdd + undoShadowDelete + undoInheritedUpdateCount + undoBackLinkUpdateCount;

            ExportAddCount -= undoExportAdds;
            ExportDeleteCount -= undoExportDelete;
            ExportReplaceCount -= undoExportReplace;
            ExportUpdateCount -= undoExportUpdate;
            ImportCount -= undoImportCount;
            ShadowAddCount -= undoShadowAdd;
            ShadowDeleteCount -= undoShadowDelete;
            InheritedUpdateCount -= undoInheritedUpdateCount;
            BackLinkUpdateCount -= undoBackLinkUpdateCount;
        }
    }
}
