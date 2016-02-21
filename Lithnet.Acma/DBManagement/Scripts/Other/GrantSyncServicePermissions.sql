DECLARE @NewUserName sysname;

SET @NewUserName = '$(LoginName)';

/* Users are typically mapped to logins, as OP's question implies, 
so make sure an appropriate login exists. */
IF NOT EXISTS(SELECT principal_id FROM sys.server_principals WHERE name = @NewUserName) BEGIN
    /* Syntax for SQL server login.  See BOL for domain logins, etc. */
    DECLARE @LoginSQL as varchar(500);
    SET @LoginSQL = 'CREATE LOGIN ['+ @NewUserName + '] FROM WINDOWS';
    EXEC (@LoginSQL);
END

/* Create the user for the specified login. */
IF NOT EXISTS(SELECT principal_id FROM sys.database_principals WHERE name = @NewUserName) BEGIN
    DECLARE @UserSQL as varchar(500);
    SET @UserSQL = 'CREATE USER [' + @NewUserName + '] FOR LOGIN [' + @NewUserName + ']';
    EXEC (@UserSQL);
END

 exec sp_addrolemember 'db_owner', @NewUserName
