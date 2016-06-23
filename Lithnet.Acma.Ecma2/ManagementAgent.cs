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
    using Lithnet.MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using Microsoft.Win32;
    using System.Runtime.ExceptionServices;
    using System.Security;
    using Lithnet.Acma.ServiceModel;

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
        /// The configuration parameters assigned to this run job
        /// </summary>
        private KeyedCollection<string, ConfigParameter> suppliedConfigParameters;

        private AcmaSyncServiceClient client = new AcmaSyncServiceClient();

        /// <summary>
        /// Initializes a new instance of the ManagementAgent class
        /// </summary>
        public ManagementAgent()
        {
        }

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

        private ImportResponse importResponse;

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
                return capabilities;
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
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.LogPath, string.Empty));
                    break;

                case ConfigParameterPage.Global:
                    break;

                case ConfigParameterPage.Partition:
                    break;

                case ConfigParameterPage.RunStep:
                    break;

                case ConfigParameterPage.Capabilities:
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
            return new ParameterValidationResult();
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
                this.client = new AcmaSyncServiceClient();
                return this.client.GetMmsSchema();
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
                Logger.WriteLine("Import data from sync engine: " + importRunStep.CustomData);

                this.client = new AcmaSyncServiceClient();
                this.client.Open();

                ImportStartRequest request = new ImportStartRequest();
                request.ImportType = importRunStep.ImportType;
                request.PageSize = 0;
                request.Schema = types;

                this.importResponse = this.client.ImportStart(request);

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

                PageRequest request = new PageRequest();
                request.PageSize = this.importPageSize;
                request.Context = this.importResponse.Context;
                Logger.WriteLine("Requesting page of {0} results",  LogLevel.Debug, this.importPageSize);
                ImportResponse page = this.client.ImportPage(request);

                Logger.WriteLine("Got page of {0} results", LogLevel.Debug, this.importPageSize);
                foreach (CSEntryChange csentry in page.Objects)
                {
                    csentries.Add(csentry);

                    if (csentry.ErrorCodeImport == MAImportError.ImportErrorCustomStopRun)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(csentry.ObjectType) || csentry.ErrorCodeImport != MAImportError.Success)
                    {
                        Logger.WriteLine("An error was detected in the following CSEntryChange object");
                        Logger.WriteLine(csentry.ToDetailString());
                    }
                }

                Logger.WriteLine("Returning page of {0} results. More to import: {1}", LogLevel.Debug, page.Objects.Count, page.HasMoreItems);

                GetImportEntriesResults importReturnInfo = new GetImportEntriesResults();
                importReturnInfo.MoreToImport = page.HasMoreItems;
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

                if (this.importResponse != null)
                {
                    results.CustomData = this.importResponse.Watermark;

                    ImportReleaseRequest request = new ImportReleaseRequest();
                    request.Context = this.importResponse.Context;
                    request.NormalTermination = importRunStep.Reason == CloseReason.Normal;
                    this.client.ImportEnd(request);
                }

                this.Close();

                Logger.WriteLine("Import Complete");
                Logger.WriteSeparatorLine('*');

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
                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Starting Export");

                this.client = new AcmaSyncServiceClient();
                this.client.Open();

                this.client.ExportStart();
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
                this.client = new AcmaSyncServiceClient();

                PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();
                IList<AttributeChange> anchorchanges = new List<AttributeChange>();
                ExportRequest request = new ExportRequest();
                request.CSEntryChanges = csentries;

                Logger.WriteLine("Exporting page of {0} objects", csentries.Count);
                ExportResponse response = this.client.ExportPage(request);
                Logger.WriteLine("Got response of {0} objects", response.Results.Count);

                foreach (CSEntryChangeResult item in response.Results)
                {
                    exportEntriesResults.CSEntryChangeResults.Add(item);
                }

                return exportEntriesResults;

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
                this.client.ExportEnd();
                this.Close();

                Logger.WriteLine("Export Complete");
                Logger.WriteSeparatorLine('*');
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        private void Close()
        {
            if (this.client != null)
            {
                if (this.client.State == System.ServiceModel.CommunicationState.Opened)
                {
                    this.client.Close();
                }

                this.client = null;
            }
        }
    }
}