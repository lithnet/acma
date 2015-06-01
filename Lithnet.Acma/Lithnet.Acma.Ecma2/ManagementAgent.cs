// -----------------------------------------------------------------------
// <copyright file="ManagementAgent.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma.Ecma2
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Acma.DataModel;
    using Lithnet.Acma;
    using Microsoft.Win32;
    using Lithnet.Fim.Core;
    using Lithnet.Common.ObjectModel;
    using System.Runtime.ExceptionServices;
    using System.Security;

    /// <summary>
    /// The IMAExtensible2 management agent called by the FIM Synchronization Service
    /// </summary>
    public class ManagementAgent :
        IMAExtensible2CallExport,
        IMAExtensible2CallImport,
        IMAExtensible2GetSchema,
        IMAExtensible2GetParameters,
        IMAExtensible2GetCapabilities
    {
        /// <summary>
        /// The import page size specified in the run step
        /// </summary>
        private int importPageSize;

        /// <summary>
        /// The enumerator used for imports
        /// </summary>
        private IEnumerator<MAObjectHologram> importEnumerator;

        /// <summary>
        /// The schema types specified for the current import run step
        /// </summary>
        private Dictionary<AcmaSchemaObjectClass, IEnumerable<AcmaSchemaAttribute>> requestedObjects = new Dictionary<AcmaSchemaObjectClass, IEnumerable<AcmaSchemaAttribute>>();

        /// <summary>
        /// The default import page size
        /// </summary>
        private int importDefaultPageSize = 500;

        /// <summary>
        /// The maximum import page size
        /// </summary>
        private int importMaxPageSize = 1000;

        /// <summary>
        /// The default export page size
        /// </summary>
        private int exportDefaultPageSize = 100;

        /// <summary>
        /// The maximum export page size
        /// </summary>
        private int exportMaxPageSize = 200;

        /// <summary>
        /// The import watermark
        /// </summary>
        private byte[] highWatermark;

        private bool useChangeTrackingForDelta = false;

        private long lastImportedChangeVersion;

        /// <summary>
        /// The configuration parameters assigned to this run job
        /// </summary>
        private KeyedCollection<string, ConfigParameter> suppliedConfigParameters;

        /// <summary>
        /// Initializes a new instance of the ManagementAgent class
        /// </summary>
        public ManagementAgent()
        {
        }

        ///// <summary>
        ///// Finalizes an instance of the ManagementAgent class
        ///// </summary>
        //~ManagementAgent()
        //{
        //    this.Dispose(false);
        //}

        /// <summary>
        /// Gets the maximum import page size
        /// </summary>
        public int ImportMaxPageSize
        {
            get
            {
                return this.importMaxPageSize;
            }
        }

        /// <summary>
        /// Gets the default import page size
        /// </summary>
        public int ImportDefaultPageSize
        {
            get
            {
                return this.importDefaultPageSize;
            }
        }

        /// <summary>
        /// Gets or sets the default export page size
        /// </summary>
        public int ExportDefaultPageSize
        {
            get
            {
                return this.exportDefaultPageSize;
            }

            set
            {
                this.exportDefaultPageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum export page size
        /// </summary>
        public int ExportMaxPageSize
        {
            get
            {
                return this.exportMaxPageSize;
            }

            set
            {
                this.exportMaxPageSize = value;
            }
        }

        /// <summary>
        /// Gets the capabilities of this management agent
        /// </summary>
        public MACapabilities Capabilities
        {
            get
            {
                MACapabilities capabilities = new MACapabilities();
                capabilities.ConcurrentOperation = true;
                capabilities.ObjectRename = false;
                capabilities.DeleteAddAsReplace = true;
                capabilities.ExportType = MAExportType.AttributeUpdate;
                capabilities.DeltaImport = true;
                capabilities.DistinguishedNameStyle = MADistinguishedNameStyle.Generic;
                capabilities.NoReferenceValuesInFirstExport = false;
                capabilities.Normalizations = MANormalizations.None;
                capabilities.IsDNAsAnchor = false;
                capabilities.SupportHierarchy = false;
                capabilities.ObjectConfirmation = MAObjectConfirmation.NoDeleteConfirmation;
                return capabilities;
            }
        }

        /// <summary>
        /// Gets the database server name
        /// </summary>
        private string ServerName
        {
            get
            {
                return this.suppliedConfigParameters[MAParameterNames.SqlServerName].Value;
            }
        }

        /// <summary>
        /// Gets the database name
        /// </summary>
        private string DatabaseName
        {
            get
            {
                return this.suppliedConfigParameters[MAParameterNames.DatabaseName].Value;
            }
        }

        private string DefaultServerName
        {
            get
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lithnet\ACMA", false);

                if (key != null)
                {
                    return key.GetValue("ServerName", "localhost") as string;
                }
                else
                {
                    return "localhost";
                }
            }
        }

        private string DefaultDatabaseName
        {
            get
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lithnet\ACMA", false);

                if (key != null)
                {
                    return key.GetValue("DatabaseName", "Lithnet.Acma") as string;
                }
                else
                {
                    return "Lithnet.Acma";
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether multithreaded exports are enabled
        /// </summary>
        private bool MultithreadedExport
        {
            get
            {
                if (this.suppliedConfigParameters.Contains(MAParameterNames.MultithreadedExport))
                {
                    return this.suppliedConfigParameters[MAParameterNames.MultithreadedExport].Value == "1" ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the location of the log file
        /// </summary>
        private string LogPath
        {
            get
            {
                return this.suppliedConfigParameters[MAParameterNames.LogPath].Value;
            }
        }

        /// <summary>
        /// Gets the path to this MA's configuration file
        /// </summary>
        private string MAConfigurationFilePath
        {
            get
            {
                return this.suppliedConfigParameters[MAParameterNames.MAConfigurationFile].Value;
            }
        }

        /// <summary>
        /// Gets the list of configuration parameters for the management agent
        /// </summary>
        /// <param name="configParameters">The list of configuration parameters</param>
        /// <param name="page">The page to get the configuration parameters for</param>
        /// <returns>A list of ConfigParameterDefinition objects</returns>
        public IList<ConfigParameterDefinition> GetConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            List<ConfigParameterDefinition> configParametersDefinitions = new List<ConfigParameterDefinition>();

            switch (page)
            {
                case ConfigParameterPage.Connectivity:
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.SqlServerName, string.Empty, this.DefaultServerName));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.DatabaseName, string.Empty, this.DefaultDatabaseName));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.MAConfigurationFile, string.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.LogPath, string.Empty));
                    break;

                case ConfigParameterPage.Global:
                    break;

                case ConfigParameterPage.Partition:
                    break;

                case ConfigParameterPage.RunStep:
                    break;
            }

            return configParametersDefinitions;
        }

        /// <summary>
        /// Validates the values provided in the UI for the supplied configuration parameters
        /// </summary>
        /// <param name="configParameters">The list of configuration parameters</param>
        /// <param name="page">The page to get the configuration parameters for</param>
        /// <returns>A ParameterValidationResult object containing the results of the validation</returns>
        public ParameterValidationResult ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            ParameterValidationResult myResults = new ParameterValidationResult();

            try
            {
                this.suppliedConfigParameters = configParameters;
                Logger.LogPath = this.LogPath;

                this.LoadConfiguration();
            }
            catch (Exception ex)
            {
                myResults.ErrorMessage = "Unable to load configuration: " + ex.Message;
                Logger.WriteException(ex);
            }

            this.Close();

            return myResults;
        }

        /// <summary>
        /// Gets the schema that applies to the objects in this management agent
        /// </summary>
        /// <param name="configParameters">The configuration parameters supplied to this management agent</param>
        /// <returns>The Schema defining objects and attributes applicable to this management agent</returns>
        public Schema GetSchema(KeyedCollection<string, ConfigParameter> configParameters)
        {
            try
            {
                this.suppliedConfigParameters = configParameters;
                this.LoadConfiguration();

                return CreateMetadirectoryServicesSchema();
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
            finally
            {
                this.Close();
            }

        }

        /// <summary>
        /// Creates the schema to present to FIM from the ACMA schema definition
        /// </summary>
        /// <returns>A Schema object to return to the FIM Sync Service</returns>
        public static Schema CreateMetadirectoryServicesSchema()
        {
            Schema schema = Schema.Create();

            foreach (AcmaSchemaObjectClass schemaObject in ActiveConfig.DB.ObjectClassesBindingList)
            {
                SchemaType schemaType = SchemaType.Create(schemaObject.Name, true);
                schemaType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute("objectId", AttributeType.String, AttributeOperation.ImportOnly));

                foreach (AcmaSchemaAttribute attribute in schemaObject.Attributes.Where(t => t.Name != "objectId"))
                {
                    if (attribute.Operation == AcmaAttributeOperation.AcmaInternal || attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
                    {
                        continue;
                    }

                    if (attribute.IsMultivalued)
                    {
                        schemaType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(attribute.Name, attribute.MmsType, ConvertAcmaAttributeOperationToAttributeOperation(attribute.Operation)));
                    }
                    else
                    {
                        schemaType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(attribute.Name, attribute.MmsType, ConvertAcmaAttributeOperationToAttributeOperation(attribute.Operation)));
                    }
                }

                schema.Types.Add(schemaType);
            }

            return schema;
        }

        /// <summary>
        /// Converts an ACMA attribute operation enumeration into its equivalent FIM AttributeOperation value
        /// </summary>
        /// <param name="operation">The ACMA operation value</param>
        /// <returns>An AttributeOperation value</returns>
        public static AttributeOperation ConvertAcmaAttributeOperationToAttributeOperation(AcmaAttributeOperation operation)
        {
            switch (operation)
            {
                case AcmaAttributeOperation.ImportExport:
                    return AttributeOperation.ImportExport;

                case AcmaAttributeOperation.ExportOnly:
                    return AttributeOperation.ExportOnly;

                case AcmaAttributeOperation.ImportOnly:
                    return AttributeOperation.ImportOnly;

                case AcmaAttributeOperation.AcmaInternal:
                case AcmaAttributeOperation.AcmaInternalTemp:
                default:
                    throw new ArgumentException(string.Format("The ACMA operation type '{0}' does not have an equivalent FIM operation type", operation.ToString()));
            }
        }

        /// <summary>
        /// Configures the import session at the beginning of an import
        /// </summary>
        /// <param name="configParameters">The configuration parameters supplied to this management agent</param>
        /// <param name="types">The schema types that apply to this import run</param>
        /// <param name="importRunStep">The definition of the current run step</param>
        /// <returns>Results of the import setup</returns>
        public OpenImportConnectionResults OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenImportConnectionRunStep importRunStep)
        {
            try
            {
                this.suppliedConfigParameters = configParameters;
                Logger.LogPath = this.LogPath;
                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Starting Import");
                Logger.WriteLine("Import data: " + importRunStep.CustomData);

                this.LoadConfiguration();

                MAStatistics.StartOperation(MAOperationType.Import);
                ActiveConfig.DB.CanCache = true;

                foreach (var item in types.Types)
                {
                    AcmaSchemaObjectClass requestedObjectClass = ActiveConfig.DB.GetObjectClass(item.Name);
                    IEnumerable<AcmaSchemaAttribute> attributes = item.Attributes.Select(t => ActiveConfig.DB.GetAttribute(t.Name));
                    this.requestedObjects.Add(requestedObjectClass, attributes);
                }

                if (importRunStep.ImportType == OperationType.Delta)
                {
                    this.ProcessOperationEvents(AcmaEventOperationType.DeltaImport);
                    this.importEnumerator = this.GetDeltaMAObjects(importRunStep, types).GetEnumerator();
                }
                else
                {
                    this.ProcessOperationEvents(AcmaEventOperationType.FullImport);
                    this.importEnumerator = this.GetFullMAObjects(importRunStep, types).GetEnumerator();
                }

                this.importPageSize = importRunStep.PageSize;

                return new OpenImportConnectionResults();
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a batch of entries from the database and returns them to the synchronization service
        /// </summary>
        /// <param name="importRunStep">The current run step</param>
        /// <returns>The results of the import batch</returns>
        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            try
            {
                List<CSEntryChange> csentries = new List<CSEntryChange>();
                int count = 0;
                bool mayHaveMore = false;

                while (this.importEnumerator.MoveNext())
                {
                    MAObjectHologram hologram = this.importEnumerator.Current;
                    // using (MAObjectHologram hologram = this.importEnumerator.Current)
                    // {
                    CSEntryChange csentry = CSEntryImport.GetCSEntry(hologram, this.requestedObjects);

                    if (csentry == null)
                    {
                        continue;
                    }

                    csentries.Add(csentry);

                    if (csentry.ErrorCodeImport == MAImportError.ImportErrorCustomStopRun)
                    {
                        this.lastImportedChangeVersion = 0;
                        break;
                    }

                    count++;

                    if (this.useChangeTrackingForDelta)
                    {
                        if (hologram.ChangeVersion > this.lastImportedChangeVersion)
                        {
                            this.lastImportedChangeVersion = hologram.ChangeVersion;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(csentry.ObjectType) || csentry.ErrorCodeImport != MAImportError.Success)
                    {
                        Logger.WriteLine("An error was detected in the following CSEntryChange object");
                        MADiagnostics.DumpCSEntryChange(csentry);
                    }

                    if (count >= this.importPageSize)
                    {
                        mayHaveMore = true;
                        break;
                    }
                    //}
                }

                GetImportEntriesResults importReturnInfo = new GetImportEntriesResults();
                importReturnInfo.MoreToImport = mayHaveMore;
                importReturnInfo.CSEntries = csentries;
                return importReturnInfo;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Closes the import session
        /// </summary>
        /// <param name="importRunStep">The current run step</param>
        /// <returns>Results of the import session close</returns>
        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStep)
        {
            try
            {
                CloseImportConnectionResults results = new CloseImportConnectionResults();

                results.CustomData = this.FinalizeImportOperation();

                this.Close();

                Logger.WriteLine("Import Complete");
                Logger.WriteSeparatorLine('*');

                MAStatistics.StopOperation();
                Logger.WriteLine(MAStatistics.ToString());

                return results;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Begins an export session
        /// </summary>
        /// <param name="configParameters">The configuration parameters supplied to this management agent</param>
        /// <param name="types">The schema types that apply to this export run</param>
        /// <param name="exportRunStep">The definition of the current run step</param>
        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            try
            {
                this.suppliedConfigParameters = configParameters;

                Logger.LogPath = this.LogPath;

                this.LoadConfiguration();
                AcmaExternalExitEvent.StartEventQueue();

                ActiveConfig.DB.CanCache = true;
                MAStatistics.StartOperation(MAOperationType.Export);
                this.ProcessOperationEvents(AcmaEventOperationType.Export);

                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Starting Export");
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Ends an export session
        /// </summary>
        /// <param name="exportRunStep">The results of the export session close</param>
        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            try
            {
                AcmaExternalExitEvent.CompleteEventQueue();
                AcmaExternalExitEvent.WaitForComplete();
                this.Close();

                Logger.WriteLine("Export Complete");
                Logger.WriteSeparatorLine('*');

                MAStatistics.StopOperation();
                Logger.WriteLine(MAStatistics.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Exports a batch of entries to the database
        /// </summary>
        /// <param name="csentries">A list of changes to export</param>
        /// <returns>The results of the batch export</returns>
        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            try
            {
                // Disabled because of reference issues with MT exports
                //if (this.MultithreadedExport)
                //{
                //    return this.PutExportEntriesParallel(csentries);
                //}
                //else
                //{
                return this.PutExportEntriesSerial(csentries);
                //}
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        private string FinalizeImportOperation()
        {
            if (this.useChangeTrackingForDelta)
            {
                return this.FinalizeImportOperationWithChangeTracking();
            }
            else
            {
                return this.FinalizeImportOperationWithRowversion();
            }
        }

        private string FinalizeImportOperationWithRowversion()
        {
            if (this.highWatermark != null)
            {
                Logger.WriteLine("Clearing delta table from watermark: " + this.highWatermark.ToSmartStringOrEmptyString());
                ActiveConfig.DB.MADataConext.ClearDeltas(this.highWatermark);
            }

            return this.highWatermark.ToSmartStringOrNull();
        }

        private string FinalizeImportOperationWithChangeTracking()
        {
            return this.lastImportedChangeVersion.ToSmartStringOrNull();
        }

        private IEnumerable<MAObjectHologram> GetDeltaMAObjects(OpenImportConnectionRunStep importRunStep, Schema types)
        {
            if (this.useChangeTrackingForDelta)
            {
                return this.GetDeltaMAObjectsFromChangeTracking(importRunStep, types);
            }
            else
            {
                return this.GetDeltaMAObjectsFromRowVersion(importRunStep, types);
            }
        }

        private IEnumerable<MAObjectHologram> GetDeltaMAObjectsFromChangeTracking(OpenImportConnectionRunStep importRunStep, Schema types)
        {
            long lastVersion = 0;

            if (!long.TryParse(importRunStep.CustomData, out lastVersion) || lastVersion == 0)
            {
                throw new Microsoft.MetadirectoryServices.ExtensibleExtensionException("A full import must be performed before a delta import");
            }

            if (lastVersion < ActiveConfig.DB.MADataConext.GetMinimumChangeVersion())
            {
                throw new Microsoft.MetadirectoryServices.ExtensibleExtensionException("The delta changes are no longer available in the database. A full import must be performed");
            }

            long currentVersion = ActiveConfig.DB.MADataConext.GetCurrentChangeVersion();

            Logger.WriteLine("Last import version: " + lastVersion);
            Logger.WriteLine("Current change version: " + currentVersion);

            if (lastVersion == currentVersion)
            {
                this.lastImportedChangeVersion = currentVersion;
                return new List<MAObjectHologram>();
            }

            return ActiveConfig.DB.MADataConext.GetDeltaMAObjects(lastVersion);
        }

        private IEnumerable<MAObjectHologram> GetDeltaMAObjectsFromRowVersion(OpenImportConnectionRunStep importRunStep, Schema types)
        {
            this.highWatermark = ActiveConfig.DB.MADataConext.GetHighWatermarkMAObjectsDelta();

            if (this.highWatermark == null)
            {
                Logger.WriteLine("No delta changes to import");
                return new List<MAObjectHologram>();
            }
            else
            {
                Logger.WriteLine("Got delta watermark: " + this.highWatermark.ToSmartStringOrEmptyString());
                return ActiveConfig.DB.MADataConext.GetDeltaMAObjects(this.highWatermark);
            }
        }

        private IEnumerable<MAObjectHologram> GetFullMAObjects(OpenImportConnectionRunStep importRunStep, Schema types)
        {
            if (this.useChangeTrackingForDelta)
            {
                return this.GetFullMAObjectsFromChangeTracking(importRunStep, types);
            }
            else
            {
                return this.GetFullMAObjectsFromRowVersion(importRunStep, types);
            }
        }

        private IEnumerable<MAObjectHologram> GetFullMAObjectsFromRowVersion(OpenImportConnectionRunStep importRunStep, Schema types)
        {
            AcmaSchemaAttribute deletedAttribute = ActiveConfig.DB.GetAttribute("deleted");
            AcmaSchemaAttribute objectClassAttribute = ActiveConfig.DB.GetAttribute("objectClass");
            this.highWatermark = ActiveConfig.DB.MADataConext.GetHighWatermarkMAObjects();

            Logger.WriteLine("Got full object watermark: " + this.highWatermark.ToSmartStringOrEmptyString());
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.DBQueries.Add(new DBQueryByValue(deletedAttribute, Fim.Core.ValueOperator.Equals, 0));
            DBQueryGroup objectClasses = new DBQueryGroup(GroupOperator.Any);

            foreach (SchemaType type in types.Types)
            {
                objectClasses.DBQueries.Add(new DBQueryByValue(objectClassAttribute, Fim.Core.ValueOperator.Equals, type.Name));
            }

            group.AddChildQueryObjects(objectClasses);


            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(group);
        }

        private IEnumerable<MAObjectHologram> GetFullMAObjectsFromChangeTracking(OpenImportConnectionRunStep importRunStep, Schema types)
        {
            AcmaSchemaAttribute deletedAttribute = ActiveConfig.DB.GetAttribute("deleted");
            AcmaSchemaAttribute objectClassAttribute = ActiveConfig.DB.GetAttribute("objectClass");
            this.lastImportedChangeVersion = ActiveConfig.DB.MADataConext.GetCurrentChangeVersion();

            Logger.WriteLine("Got full import current change version: " + this.lastImportedChangeVersion);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.DBQueries.Add(new DBQueryByValue(deletedAttribute, Fim.Core.ValueOperator.Equals, 0));
            DBQueryGroup objectClasses = new DBQueryGroup(GroupOperator.Any);

            foreach (SchemaType type in types.Types)
            {
                objectClasses.DBQueries.Add(new DBQueryByValue(objectClassAttribute, Fim.Core.ValueOperator.Equals, type.Name));
            }

            group.AddChildQueryObjects(objectClasses);

            return ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(group);
        }

        /// <summary>
        /// Exports a batch of entries using multiple threads
        /// </summary>
        /// <param name="csentries">A list of changes to export</param>
        /// <returns>The results of the batch export</returns>
        private PutExportEntriesResults PutExportEntriesParallel(IList<CSEntryChange> csentries)
        {
            PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();
            IList<AttributeChange> anchorchanges = new List<AttributeChange>();
            BlockingCollection<CSEntryChangeResult> changeResults = new BlockingCollection<CSEntryChangeResult>();

            Parallel.ForEach(
                csentries,
                csentryChange =>
                {
                    try
                    {
                        Logger.StartThreadLog();
                        Logger.WriteSeparatorLine('-');

                        using (MADataContext threadContext = new MADataContext(this.ServerName, this.DatabaseName))
                        {
                            bool referenceRetryRequired;
                            anchorchanges = CSEntryExport.PutExportEntry(csentryChange, threadContext, out referenceRetryRequired);

                            if (referenceRetryRequired)
                            {
                                Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentryChange.DN));
                                changeResults.Add(CSEntryChangeResult.Create(csentryChange.Identifier, anchorchanges, MAExportError.ExportActionRetryReferenceAttribute));
                            }
                            else
                            {
                                changeResults.Add(CSEntryChangeResult.Create(csentryChange.Identifier, anchorchanges, MAExportError.Success));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(ex);
                        MAStatistics.AddExportError();
                        changeResults.Add(GetExportChangeResultFromException(csentryChange, ex));
                    }
                    finally
                    {
                        Logger.EndThreadLog();
                    }
                });

            foreach (CSEntryChangeResult result in changeResults)
            {
                exportEntriesResults.CSEntryChangeResults.Add(result);
            }

            return exportEntriesResults;
        }

        /// <summary>
        /// Exports a batch of entries using a single thread
        /// </summary>
        /// <param name="csentries">A list of changes to export</param>
        /// <returns>The results of the batch export</returns>
        private PutExportEntriesResults PutExportEntriesSerial(IList<CSEntryChange> csentries)
        {
            PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();
            IList<AttributeChange> anchorchanges = new List<AttributeChange>();

            foreach (CSEntryChange csentryChange in csentries)
            {
                int currentExportCount = MAStatistics.ExportCount;
                int currentInheritedCount = MAStatistics.InheritedUpdateCount;
                int currentShadowAddCount = MAStatistics.ShadowAddCount;
                int currentShadowDeleteCount = MAStatistics.ShadowDeleteCount;

                try
                {
                    bool referenceRetryRequired;
                    anchorchanges = CSEntryExport.PutExportEntry(csentryChange, ActiveConfig.DB.MADataConext, out referenceRetryRequired);

                    if (referenceRetryRequired)
                    {
                        Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentryChange.DN));
                        exportEntriesResults.CSEntryChangeResults.Add(CSEntryChangeResult.Create(csentryChange.Identifier, anchorchanges, MAExportError.ExportActionRetryReferenceAttribute));
                    }
                    else
                    {
                        exportEntriesResults.CSEntryChangeResults.Add(CSEntryChangeResult.Create(csentryChange.Identifier, anchorchanges, MAExportError.Success));
                    }
                }
                catch (Exception ex)
                {
                    MAStatistics.AddExportError();

                    if (exportEntriesResults.CSEntryChangeResults.Contains(csentryChange.Identifier))
                    {
                        exportEntriesResults.CSEntryChangeResults.Remove(csentryChange.Identifier);
                    }

                    exportEntriesResults.CSEntryChangeResults.Add(this.GetExportChangeResultFromException(csentryChange, ex));
                }
            }

            return exportEntriesResults;
        }

        /// <summary>
        /// Constructs a CSEntryChangeResult object appropriate to the specified exception
        /// </summary>
        /// <param name="csentryChange">The CSEntryChange object that triggered the exception</param>
        /// <param name="ex">The exception that was caught</param>
        /// <returns>A CSEntryChangeResult object with the correct error code for the exception that was encountered</returns>
        private CSEntryChangeResult GetExportChangeResultFromException(CSEntryChange csentryChange, Exception ex)
        {
            if (ex is NoSuchObjectException)
            {
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportErrorConnectedDirectoryMissingObject);
            }
            else if (ex is ReferencedObjectNotPresentException)
            {
                Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentryChange.DN));
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportActionRetryReferenceAttribute);
            }
            else
            {
                Logger.WriteLine(string.Format("An unexpected exception occurred for csentry change {0} with DN {1}. ", csentryChange.Identifier.ToString(), csentryChange.DN ?? string.Empty));
                Logger.WriteException(ex);
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportErrorCustomContinueRun, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Loads the MA configuration
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                this.OpenDataContext();

                if (string.IsNullOrWhiteSpace(this.MAConfigurationFilePath))
                {
                    Logger.WriteLine("No configuration file specified. Loading MA with no rules");
                    ActiveConfig.XmlConfig = new XmlConfigFile();
                }
                else
                {
                    Logger.WriteLine("Loading configuration file {0}", this.MAConfigurationFilePath);

                    if (ActiveConfig.XmlConfig == null || ActiveConfig.XmlConfig.FileName != this.MAConfigurationFilePath)
                    {
                        ActiveConfig.LoadXml(this.MAConfigurationFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("An error occurred while loading the MA");
                Logger.WriteException(ex);
                throw;
            }
        }

        private void ProcessOperationEvents(AcmaEventOperationType operationType)
        {
            AcmaEvents events = ActiveConfig.XmlConfig.OperationEvents;
            if (events == null || events.Count == 0)
            {
                return;
            }

            Logger.WriteSeparatorLine('-');
            Logger.WriteLine("Processing pre-operation events");

            foreach (AcmaOperationEvent e in events.OfType<AcmaOperationEvent>().Where(t => t.OperationTypes.HasFlag(operationType)))
            {
                foreach (MAObjectHologram hologram in e.GetQueryRecipients(ActiveConfig.DB.MADataConext))
                {
                    Logger.WriteLine("Sending event {0} to {1}", e.ID, hologram.DisplayText);
                    try
                    {
                        Logger.IncreaseIndent();
                        hologram.ProcessEvents(new List<RaisedEvent>() { new RaisedEvent(e) });
                    }
                    finally
                    {
                        Logger.DecreaseIndent();
                    }
                }
            }

            Logger.WriteLine("Event distribution completed");
            Logger.WriteSeparatorLine('-');
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void OpenDataContext()
        {
            try
            {
                Logger.WriteLine("Opening connection to database {0}\\{1}", this.ServerName, this.DatabaseName);
                ActiveConfig.OpenDatabase(this.ServerName, this.DatabaseName);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        private void Close()
        {
            if (ActiveConfig.XmlConfig != null)
            {
                ActiveConfig.XmlConfig = null;
            }

            if (ActiveConfig.DB != null)
            {
                // This prevents a bug in .NET where something in the connection pool is getting disposed and attempted to be
                // reused. This causes the sync engine to crash intermittently when starting an option. The sync engine does not
                // unload the AppDomain where the MA is loaded for 5 minutes after the operation finishes. Running the MA out-of-process
                // does not cause this problem, as the AppDomain is terminated when dllhost.exe terminates.

                SqlConnection.ClearPool(ActiveConfig.DB.MADataConext.SqlConnection);

                ActiveConfig.DB = null;
            }

            if (this.importEnumerator != null)
            {
                this.importEnumerator.Dispose();
                this.importEnumerator = null;
            }
        }
    }
}