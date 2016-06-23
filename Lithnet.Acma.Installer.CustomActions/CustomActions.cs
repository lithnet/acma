using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.DirectoryServices.AccountManagement;

namespace Lithnet.Acma.Installer
{
    public class CustomActions
    {
        private const string GroupNameAcmaAdministrators = "AcmaAdministrators";
        
        private const string GroupNameAcmaSyncUsers = "AcmaSyncUsers";

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
            string acmaServiceAccount = session.CustomActionData["AcmaServiceAccount"];

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
                    upgrader.GrantDBRights(dbName, acmaServiceAccount);
                    session.Log("Database is up to date");
                }
            }
            else
            {
                session.Log("Database {0} doesn't exist. Creating and assigning permissions to sync engine account {1}", dbName, acmaServiceAccount);
                upgrader.CreateDB(dbName, acmaServiceAccount);
                session.Log("Database creation complete");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CreateAcmaGroups(Session session)
        {
            CustomActions.CreateAcmaAdministratorsGroup(session);

            //string syncServiceAccount = session.CustomActionData["SyncServiceAccount"];

            //if (syncServiceAccount == null)
            //{
            //    session.Log("The sync service account parameter was not provided");
            //    throw new ArgumentException("The sync service account parameter was not provided");
            //}

            //CustomActions.CreateAcmaSyncUsersGroup(session, syncServiceAccount);

            return ActionResult.Success;
        }

        private static void CreateAcmaSyncUsersGroup(Session session, string syncAccount)
        {
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, CustomActions.GroupNameAcmaSyncUsers);
            bool mustSave = false;

            if (group == null)
            {
                session.Log("Creating new group {0}", CustomActions.GroupNameAcmaSyncUsers);
                group = new GroupPrincipal(context);
                group.Name = CustomActions.GroupNameAcmaSyncUsers;
                mustSave = true;
            
            }

            UserPrincipal user = CustomActions.FindInDomainOrMachine(syncAccount);

            if (user == null)
            {
                session.Log("User not found {0}", syncAccount);
                throw new NoMatchingPrincipalException(string.Format("The user {0} could not be found", syncAccount));
            }


            if (!group.Members.Contains(user))
            {
                session.Log("Added user {0} to group {1}", syncAccount, CustomActions.GroupNameAcmaSyncUsers);

                group.Members.Add(user);
                mustSave = true;
            }

            if (mustSave)
            {
                group.Save();
            }
        }

        private static UserPrincipal FindInDomainOrMachine(string accountName)
        {
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, accountName);

            if (user == null)
            {
                context = new PrincipalContext(ContextType.Machine);
                user = UserPrincipal.FindByIdentity(context, accountName);
            }

            return user;
        }

        private static void CreateAcmaAdministratorsGroup(Session session)
        {
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, CustomActions.GroupNameAcmaAdministrators);

            if (group == null)
            {
                session.Log("Creating new group {0}", CustomActions.GroupNameAcmaSyncUsers);

                group = new GroupPrincipal(context);
                group.Name = CustomActions.GroupNameAcmaAdministrators;
                group.Save();
            }
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
