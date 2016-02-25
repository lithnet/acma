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
//using Lithnet.Acma.DataModel;

namespace Lithnet.Acma
{
    public class DBInstallUpgrader
    {
        private static int dbMajorVersion = 1;

        private static int dbMinorVersion = 7;

        private static int dbRevisionVersion = 2;

        private string ServerName { get; set; }

        private SqlConnection sqlConnection;

        private SqlConnection SqlConnection
        {
            get
            {
                if (this.sqlConnection == null)
                {
                    this.sqlConnection = new SqlConnection(string.Format("Server={0};Database=master;Integrated Security=True;", this.ServerName));
                    
                    this.sqlConnection.Open();
                }
                else
                {
                    if (this.sqlConnection.State == ConnectionState.Closed)
                    {
                        this.sqlConnection.Open();
                    }
                }

                return this.sqlConnection;
            }
        }

        public void ValidateSqlServerVersion()
        {
            string serverVersion = this.SqlConnection.ServerVersion;
            string[] serverVersionDetails = serverVersion.Split(new string[] { "." }, StringSplitOptions.None);

            int versionNumber = int.Parse(serverVersionDetails[0]);

            if (versionNumber < 11)
            {
                throw new InvalidOperationException("SQL Server version 2012 or above is required");
            }
        }

        public static Version DBVersion
        {
            get
            {
                return new Version(DBInstallUpgrader.dbMajorVersion, DBInstallUpgrader.dbMinorVersion, DBInstallUpgrader.dbRevisionVersion);
            }
        }

        public DBInstallUpgrader(string serverName)
        {
            this.ServerName = serverName;
        }

        internal static void ValidateVersion(int major, int minor, int revision)
        {
            if (major < DBInstallUpgrader.dbMajorVersion)
            {
                throw new DBVersionException(strings.DBUpdateRequired);

            }
            else if (major > DBInstallUpgrader.dbMajorVersion)
            {
                throw new DBVersionException(strings.ApplicationUpdateRequired);
            }
            else
            {
                // if (version.MajorReleaseNumber == this.dbMajorVersion)

                if (minor > DBInstallUpgrader.dbMinorVersion)
                {
                    throw new DBVersionException(strings.ApplicationUpdateRequired);
                }
                else if (minor < DBInstallUpgrader.dbMinorVersion)
                {
                    throw new DBVersionException(strings.DBUpdateRequired);
                }
                else
                {
                    // if (version.MinorReleaseNumber == this.MinorReleaseNumber)
                    if (revision < DBInstallUpgrader.dbRevisionVersion)
                    {
                        throw new DBVersionException(strings.DBUpdateRequired);
                    }
                }
            }
        }

        public bool DoesDatabaseExist(string databaseName)
        {
            this.ValidateSqlServerVersion();

            try
            {
                string selectDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);

                using (SqlCommand sqlCmd = new SqlCommand(selectDBQuery, this.SqlConnection))
                {
                    object resultObj = sqlCmd.ExecuteScalar();

                    int databaseID = 0;

                    if (resultObj != null)
                    {
                        int.TryParse(resultObj.ToString(), out databaseID);
                    }

                    return databaseID > 0;
                }

            }
            catch
            {
                return false;
            }
        }

        public Version GetDBVersion(string databaseName)
        {
            string selectDBQuery = string.Format("SELECT TOP 1 * FROM [{0}].[dbo].[DB_Version] ORDER BY MajorReleaseNumber DESC, MinorReleaseNumber DESC, PointReleaseNumber DESC", databaseName);

            SqlCommand command = new SqlCommand();
            command.Connection = this.SqlConnection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = selectDBQuery;

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;

            if (adapter.Fill(dataset) == 0)
            {
                throw new DataException("An error occurred while trying to obtain the database version");
            }
            else
            {
                DataRow row = dataset.Tables[0].Rows[0];
                return new Version((int)row["MajorReleaseNumber"], (int)row["MinorReleaseNumber"], (int)row["PointReleaseNumber"]);
            }
        }

        public void CreateDB(string databaseName)
        {
            this.CreateDB(databaseName, null);
        }

        public void CreateDB(string databaseName, string syncServiceAccount)
        {
            this.ValidateSqlServerVersion();
            string createScript = this.GetSqlScript("DBManagement.Scripts.New.CreateDB.sql");
            this.ExecuteSql(databaseName, createScript, "master", false);

            if (!string.IsNullOrWhiteSpace(syncServiceAccount))
            {
                string grantScript = this.GetSqlScript("DBManagement.Scripts.Other.GrantSyncServicePermissions.sql");
                grantScript = grantScript.Replace("$(LoginName)", syncServiceAccount.Replace(@"\\", @"\"));
                this.ExecuteSql(databaseName, grantScript, databaseName, true);
            }
        }

        public void UpgradeDB(string databaseName)
        {
            this.ValidateSqlServerVersion();
            Version dbVersion = this.GetDBVersion(databaseName);

            if (dbVersion == new Version(1, 5, 3))
            {
                this.UpgradeTo1_5_4(databaseName);
            }
            else if (dbVersion == new Version(1, 5, 4))
            {
                this.UpgradeTo1_5_5(databaseName);
            }
            else if (dbVersion == new Version(1, 5, 5))
            {
                this.UpgradeTo1_6_0(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 0))
            {
                this.UpgradeTo1_6_1(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 1))
            {
                this.UpgradeTo1_6_2(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 2))
            {
                this.UpgradeTo1_6_3(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 3))
            {
                this.UpgradeTo1_6_4(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 4))
            {
                this.UpgradeTo1_6_5(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 5))
            {
                this.UpgradeTo1_6_6(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 6))
            {
                this.UpgradeTo1_6_7(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 7))
            {
                this.UpgradeTo1_6_8(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 8))
            {
                this.UpgradeTo1_6_9(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 9))
            {
                this.UpgradeTo1_6_10(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 10))
            {
                this.UpgradeTo1_6_11(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 11))
            {
                this.UpgradeTo1_6_12(databaseName);
            }
            else if (dbVersion == new Version(1, 6, 12))
            {
                this.UpgradeTo1_7_0(databaseName);
            }
            else if (dbVersion == new Version(1, 7, 0))
            {
                this.UpgradeTo1_7_1(databaseName);
            }
            else if (dbVersion == new Version(1, 7, 1))
            {
                this.UpgradeTo1_7_2(databaseName);
            }
            else if (dbVersion == DBInstallUpgrader.DBVersion)
            {
                // Upgrade done
                return;
            }
            else
            {
                throw new DBVersionException("The database could not be updated to the latest version as the installation routine did not have the appropriate upgrade script");
            }

            this.UpgradeDB(databaseName);
        }

        private void UpgradeTo1_5_4(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_5_4.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_5_5(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_5_5.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_6_0(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_0.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_6_1(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_1.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_6_2(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_2.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_6_3(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_3.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_6_4(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_4.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, true);
        }

        private void UpgradeTo1_6_5(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_5.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_6(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_6.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_7(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_7.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_8(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_8.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_9(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_9.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_10(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_10.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_11(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_11.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_6_12(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_6_12.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_7_0(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_7_0.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_7_1(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_7_1.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private void UpgradeTo1_7_2(string databaseName)
        {
            string createScript = this.GetSqlScript("DBManagement.Scripts.Upgrades.1_7_2.sql");
            this.ExecuteSql(databaseName, createScript, databaseName, false);
        }

        private string GetSqlScript(string scriptName)
        {
            try
            {
                // Gets the current assembly.
                Assembly assembly = Assembly.GetExecutingAssembly();

                string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(t => t.EndsWith(scriptName));

                if (string.IsNullOrWhiteSpace(resourceName))
                {
                    throw new FileNotFoundException(string.Format("The script file {0} was not found in the assembly", scriptName));
                }

                // Resources are named using a fully qualified name.
                Stream strm = assembly.GetManifestResourceStream(resourceName);

                // Reads the contents of the embedded file.
                StreamReader reader = new StreamReader(strm);
                return reader.ReadToEnd();

            }
            catch (Exception ex)
            {
                throw new FileNotFoundException(string.Format("Could not locate the script {0} in the resource", scriptName), ex);
            }
        }

        private void ExecuteSql(string targetDatabase, string sql, string connectionDatabase, bool transact)
        {
            string createScript = sql.Replace("$(DatabaseName)", targetDatabase);
            string[] splitSql = Regex.Split(createScript, @"^GO\r\n", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            if (transact)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                {
                    this.SqlConnection.EnlistTransaction(Transaction.Current);
                    ExecuteSqlStatements(connectionDatabase, splitSql);
                    scope.Complete();
                }
            }
            else
            {
                ExecuteSqlStatements(connectionDatabase, splitSql);
            }
        }

        private void ExecuteSqlStatements(string connectionDatabase, string[] commands)
        {
            if (connectionDatabase != null)
            {
                this.SqlConnection.ChangeDatabase(connectionDatabase);
            }

            foreach (string commandText in commands)
            {
                SqlCommand sqlCommand = new SqlCommand(commandText, this.SqlConnection);
                sqlCommand.CommandTimeout = 0;

                if (string.IsNullOrWhiteSpace(commandText))
                {
                    continue;
                }
                try
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Executing:\n{0}", commandText));
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new InstallException("The SQL script failed to execute:" + ex.Message, ex);
                }
            }
        }
    }
}
