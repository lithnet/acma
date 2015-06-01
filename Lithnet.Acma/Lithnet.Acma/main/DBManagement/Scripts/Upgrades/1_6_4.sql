PRINT N'Altering [dbo].[spSchemaCreateIndex]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Creates an index on the MA_Objects table
-- =============================================
ALTER PROCEDURE [dbo].[spSchemaCreateIndex]
    @attributeName nvarchar(50) 
AS
BEGIN
    DECLARE @sql nvarchar(4000);
    SET NOCOUNT ON;

    DECLARE @operation int;
    DECLARE @tableName nvarchar(50);

    SELECT  @operation = [dbo].[MA_SchemaAttributes].[Operation],
            @tableName = [dbo].[MA_SchemaAttributes].[TableName]
    FROM [dbo].[MA_SchemaAttributes] WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName;
    
    IF (@tableName != 'MA_Objects')
        THROW 50010, N'Cannot modify the index of an object that is not in the MA_Objects table', 1;

    IF (@operation = 4)
        THROW 50011, N'Cannot create an index on a temporary attribute',1;
        
    BEGIN TRANSACTION
        EXEC [dbo].[spSchemaDeleteIndex] @attributeName;
        SET @sql = N'CREATE NONCLUSTERED INDEX IX_MA_Objects_' + @attributeName + N' ON [dbo].[MA_Objects] (' + @attributeName + N') 
                WITH (STATISTICS_NORECOMPUTE = OFF, 
                        IGNORE_DUP_KEY = OFF, 
                        ALLOW_ROW_LOCKS = ON, 
                        ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]'
        EXEC sp_executesql @sql

        UPDATE dbo.MA_SchemaAttributes
        SET [dbo].[MA_SchemaAttributes].[IsIndexed] = 1
        WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName

    COMMIT TRANSACTION
END
GO
PRINT N'Altering [dbo].[spSchemaRenameAttribute]...';


GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 30-05-2014
-- Description:	Renames an attribute in the schema
-- =============================================
ALTER PROCEDURE [dbo].[spSchemaRenameAttribute]
    @attributeName nvarchar(50),
    @newAttributeName nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        SET QUOTED_IDENTIFIER ON
        SET ARITHABORT ON
        SET NUMERIC_ROUNDABORT OFF
        SET CONCAT_NULL_YIELDS_NULL ON
        SET ANSI_NULLS ON
        SET ANSI_PADDING ON
        SET ANSI_WARNINGS ON
    COMMIT

    DECLARE @isMultiValued bit;
    DECLARE @isIndexed bit;
    DECLARE @tableName nvarchar(50);
    DECLARE @type int;

    SELECT 
        @isIndexed = [dbo].[MA_SchemaAttributes].[IsIndexed],
        @tableName = [dbo].[MA_SchemaAttributes].[TableName],
        @type = [dbo].[MA_SchemaAttributes].[Type]
    FROM [dbo].[MA_SchemaAttributes] WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName;

    BEGIN TRANSACTION
        IF (@tableName = 'MA_Objects')
            BEGIN
                IF (@isIndexed = 1)
                    EXEC [dbo].spSchemaDeleteIndex @attributeName;

                DECLARE @columnName nvarchar(150);
                SET @columnName = 'MA_Objects.' + @attributeName;
                EXEC [dbo].[sp_rename] @columnName, @newAttributeName, 'COLUMN';
            
                IF (@type = 4)
                BEGIN
                    declare @constraintName nvarchar(100) = N'[DF_MA_Objects_' + @attributeName + ']'
                    IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@constraintName) AND type = 'D')
                        BEGIN
                            DECLARE @oldDefaultName nvarchar(100) = N'DF_MA_Objects_' + @attributeName;
                            DECLARE @newDefaultName nvarchar(100) = N'DF_MA_Objects_' + @newAttributeName;
                            exec sp_rename @oldDefaultName, @newDefaultName, 'object'
                        END
                END

                IF (@isIndexed = 1)
                    EXEC [dbo].spSchemaCreateIndex @newAttributeName;

                UPDATE [dbo].[MA_SchemaAttributes]
                SET 
                    [dbo].[MA_SchemaAttributes].[ColumnName] = @newAttributeName,
                    [dbo].[MA_SchemaAttributes].[Name] = @newAttributeName
                WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName
            END
        ELSE
            BEGIN
                UPDATE [dbo].[MA_SchemaAttributes]
                SET [dbo].[MA_SchemaAttributes].[Name] = @newAttributeName
                WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName
            END

    COMMIT TRANSACTION
END
GO
PRINT N'Altering [dbo].[spSchemaDeleteAttribute]...';


GO

-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Deletes an attribute from the schema
-- =============================================
ALTER PROCEDURE [dbo].[spSchemaDeleteAttribute]
    @ID int
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        SET QUOTED_IDENTIFIER ON
        SET ARITHABORT ON
        SET NUMERIC_ROUNDABORT OFF
        SET CONCAT_NULL_YIELDS_NULL ON
        SET ANSI_NULLS ON
        SET ANSI_PADDING ON
        SET ANSI_WARNINGS ON
    COMMIT
    /*
        String = 0,
        Integer = 1,
        Reference = 2,
        Binary = 3,
        Boolean = 4,
        Undefined = 5,
        DateTime = 6
    */

    DECLARE @tableName nvarchar(50);
    DECLARE @columnName nvarchar(50);

    DECLARE @isBuiltIn bit;
    DECLARE @isIndexed bit;
    DECLARE @name nvarchar(50);
    DECLARE @count int;
    DECLARE @type int;

    SELECT TOP 1
        @count = 1,
        @isBuiltIn = [dbo].[MA_SchemaAttributes].[IsBuiltIn],
        @name = [dbo].[MA_SchemaAttributes].[Name],
        @type = [dbo].[MA_SchemaAttributes].[Type],
        @isIndexed = [dbo].[MA_SchemaAttributes].[IsIndexed],
        @tableName = [dbo].[MA_SchemaAttributes].[TableName],
        @columnName = [dbo].[MA_SchemaAttributes].[ColumnName]
    FROM [dbo].[MA_SchemaAttributes]
    WHERE [dbo].[MA_SchemaAttributes].[ID] = @ID

    IF (@count is null)
        THROW 50009, N'Attribute not found', 1;

    IF (@isBuiltIn is null)
        THROW 50020, N'Cannot delete a built-in attribute', 1;
 
    DECLARE @sql nvarchar(4000);

    BEGIN TRANSACTION
    
        IF (@tableName = 'MA_Objects')
            BEGIN
                IF (@isIndexed = 1)
                    EXEC [dbo].[spSchemaDeleteIndex] @columnName

                IF (@type = 4)
                    BEGIN
                    declare @constraintName nvarchar(100) = N'[DF_MA_Objects_' + @name + ']'
                    IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@constraintName) AND type = 'D')
                        BEGIN    
                            SET @sql = 'ALTER TABLE [dbo].MA_Objects DROP CONSTRAINT [' + @constraintName + ']'
                            EXEC sp_executesql @sql
                        END
                    END

                DECLARE @hasColumn bit;
                EXEC @hasColumn = [dbo].[spSchemaHasColumn] @columnName;

                if (@hasColumn = 1)
                    BEGIN
                        SET @sql = N'ALTER TABLE [dbo].[MA_Objects] DROP COLUMN ' + @columnName;
                        EXEC sp_executesql @sql;
                        ALTER TABLE [dbo].[MA_Objects] SET (LOCK_ESCALATION = TABLE);
                    END
            END
        ELSE
            BEGIN
                SET @sql = N'DELETE FROM [dbo].[' + @tableName + '] where attributeName = @name;'
            END


        DELETE FROM [dbo].[MA_SchemaAttributes]
            WHERE [dbo].[MA_SchemaAttributes].[ID] = @ID;
    COMMIT
END
GO

PRINT N'Refreshing [dbo].[spSchemaSetupNewAttribute]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spSchemaSetupNewAttribute]';


INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 4, 'Fixes to SPs related to attribute modifications', GETDATE())
GO

GO
PRINT N'Update complete.';
