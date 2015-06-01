using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Lithnet.Acma
{
    [RunInstaller(true)]
    public partial class DBInstaller : System.Configuration.Install.Installer
    {
        private string SqlServerName
        {
            get
            {
                return this.Context.Parameters["server"];
            }
        }

        private string DatabaseName
        {
            get
            {
                return this.Context.Parameters["database"];
            }
        }

        private string SyncServiceAccount
        {
            get
            {
                return this.Context.Parameters["account"];
            }
        }

        public DBInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            Logging.Logger.LogPath = System.Environment.ExpandEnvironmentVariables(@"%temp%\acma-db-install.log");
            Logging.Logger.LogLevel = Logging.LogLevel.Debug;
            Logging.Logger.WriteLine("Parameter sqlservername: " + this.SqlServerName);
            Logging.Logger.WriteLine("Parameter databasename: " + this.DatabaseName);
            Logging.Logger.WriteLine("Parameter syncserviceaccount: " + this.SyncServiceAccount);

            this.ValidateParameters();

            try
            {
                DBInstallUpgrader upgrader = new DBInstallUpgrader(this.SqlServerName);

                if (upgrader.DoesDatabaseExist(this.DatabaseName))
                {
                    Version dbVersion = upgrader.GetDBVersion(this.DatabaseName);

                    if (dbVersion < DBInstallUpgrader.DBVersion)
                    {
                        upgrader.UpgradeDB(this.DatabaseName);
                    }
                    else if (dbVersion > DBInstallUpgrader.DBVersion)
                    {
                        throw new DBVersionException("This version of ACMA cannot be installed because the database is of a newer version than the application. Obtain the latest source files and try again");
                    }
                }
                else
                {
                    upgrader.CreateDB(this.DatabaseName, this.SyncServiceAccount);
                }

                base.Install(stateSaver);
            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
                throw;
            }
        }

        private void ValidateParameters()
        {
            if (string.IsNullOrWhiteSpace(this.SqlServerName))
            {
                throw new ArgumentNullException("SQLSERVERNAME", "The SQL server name was not provided to the installation routine");
            }

            if (string.IsNullOrWhiteSpace(this.DatabaseName))
            {
                throw new ArgumentNullException("DBNAME", "The database name was not provided to the installation routine");
            }
        }
    }
}
