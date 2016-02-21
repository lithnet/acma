
-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Deletes an attribute from the schema
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaDeleteAttribute]
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
							SET @sql = 'ALTER TABLE [dbo].MA_Objects DROP CONSTRAINT ' + @constraintName; 
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