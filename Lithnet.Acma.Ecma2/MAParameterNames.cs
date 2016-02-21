// -----------------------------------------------------------------------
// <copyright file="MAParameterNames.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma.Ecma2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains a list of parameter names used in the MA
    /// </summary>
    public static class MAParameterNames
    {
        /// <summary>
        /// The SQL server name parameter name
        /// </summary>
        public const string SqlServerName = "SQL Server";

        /// <summary>
        /// The database name parameter name
        /// </summary>
        public const string DatabaseName = "Database Name";

        /// <summary>
        /// The multi-threaded export parameter name
        /// </summary>
        public const string MultithreadedExport = "Enable multithreaded export";

        /// <summary>
        /// The configuration file parameter name
        /// </summary>
        public const string MAConfigurationFile = "MA configuration file";

        /// <summary>
        /// The log file path parameter name
        /// </summary>
        public const string LogPath = "Log path";
    }
}
