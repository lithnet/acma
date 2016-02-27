using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace Lithnet.Acma.Installer
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult ReplaceModuleVariable(Session session)
        {
            string psModuleFile = session.CustomActionData["PSModulePath"];
            string installDir = session.CustomActionData["InstallDir"];

            string file = File.ReadAllText(psModuleFile).Replace("!EDITORPATH!", installDir);
            File.WriteAllText(psModuleFile, file);

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult InstallUpgradeDB(Session session)
        {
            string sqlServerName = session.CustomActionData["SqlServerName"];
            string dbName = session.CustomActionData["DBName"];
            string syncServiceAccount = session.CustomActionData["SyncServiceAccount"];

            ValidateInstallUpgradeDBParameters(sqlServerName, dbName);

            DBInstallUpgrader upgrader = new DBInstallUpgrader(sqlServerName);

            if (upgrader.DoesDatabaseExist(dbName))
            {
                session.Log("Database {0} already exists", dbName);
                Version dbVersion = upgrader.GetDBVersion(dbName);
                session.Log("Database version: {0}", dbVersion);

                if (dbVersion < DBInstallUpgrader.DBVersion)
                {
                    session.Log("Database update required to: {0}", DBInstallUpgrader.DBVersion);
                    upgrader.UpgradeDB(dbName);
                    session.Log("Database update complete");
                }
                else if (dbVersion > DBInstallUpgrader.DBVersion)
                {
                    session.Log("Database is newer than binary version: {0}", DBInstallUpgrader.DBVersion);
                    throw new DBVersionException("This version of ACMA cannot be installed because the database is of a newer version than the application. Obtain the latest source files and try again");
                }
                else
                {
                    session.Log("Database is up to date");
                }
            }
            else
            {
                session.Log("Database {0} doesn't exist. Creating and assigning permissions to sync engine account {1}", dbName, syncServiceAccount);
                upgrader.CreateDB(dbName, syncServiceAccount);
                session.Log("Database creation complete");
            }

            return ActionResult.Success;
        }

        private static void ValidateInstallUpgradeDBParameters(string servername, string databasename)
        {
            if (string.IsNullOrWhiteSpace(servername))
            {
                throw new ArgumentNullException("SQLSERVERNAME", "The SQL server name was not provided to the installation routine");
            }

            if (string.IsNullOrWhiteSpace(databasename))
            {
                throw new ArgumentNullException("DBNAME", "The database name was not provided to the installation routine");
            }
        }
    }
}
