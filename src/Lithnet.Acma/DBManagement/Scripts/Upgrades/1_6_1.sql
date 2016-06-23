-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Configures a new attribute in the schema
-- =============================================
ALTER PROCEDURE [dbo].[spSchemaSetupNewAttribute]
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
        Undefined = 5
        DateTime = 99,
    */

    DECLARE @dataType nvarchar(30);
    DECLARE @tableName nvarchar(50);
    DECLARE @columnName nvarchar(50);

    DECLARE @operation int;
    DECLARE @isMultiValued bit;
    DECLARE @type int;
    DECLARE @isIndexed bit;
    DECLARE @isIndexable bit;
    DECLARE @name nvarchar(50);
    DECLARE @count int;
    
    SELECT TOP 1
        @count = 1,
        @name = [dbo].[MA_SchemaAttributes].[Name],
        @operation = [dbo].[MA_SchemaAttributes].[Operation],
        @isMultiValued = [dbo].[MA_SchemaAttributes].[IsMultivalued],
        @type = [dbo].[MA_SchemaAttributes].[Type],
        @isIndexable = [dbo].[MA_SchemaAttributes].[IsIndexable],
        @isIndexed = [dbo].[MA_SchemaAttributes].[IsIndexed]
    FROM [dbo].[MA_SchemaAttributes]
    WHERE [dbo].[MA_SchemaAttributes].[ID] = @ID

    IF (@count is null)
        THROW 50009, N'Attribute not found', 1;
 
    IF (@operation = 4) -- AcmaInternalTemp
        BEGIN
            SET @isIndexable = 0;
            SET @isIndexed = 0;
        END
    
    IF (@type = 2)
        SET @tableName = 'MA_References';
    ELSE IF (@isMultiValued = 1)
        SET @tableName = 'MA_Attributes';
    ELSE
        SET @tableName = 'MA_Objects';

    IF (@type = 0) -- String
        BEGIN
            IF (@isIndexable = 1)
                SET @dataType = N'nvarchar(400)';
            ELSE
                SET @dataType = N'nvarchar(MAX)';
            
            IF (@isMultiValued = 1)
                IF (@isIndexable = 1)
                    SET @columnName = N'attributeValueStringIX';
                ELSE
                    SET @columnName = N'attributeValueString';
            ELSE
                SET @columnName = @name;
        END

    ELSE IF (@type = 1) --Integer
        BEGIN
            SET @dataType = 'bigint';

            IF (@isMultiValued = 1)
                SET @columnName = N'attributeValueInt';
            ELSE
                SET @columnName = @name;

            SET @isIndexable = 1;
        END

    ELSE IF (@type = 2) -- Reference
        BEGIN
            SET @dataType = 'uniqueidentifier'
            SET @isIndexable = 1
            SET @isIndexed = 1
            SET @columnName = N'Value';
        END

    ELSE IF (@type = 3) -- Binary
        BEGIN 
            IF (@isIndexable = 1)
                SET @dataType = N'varbinary(800)'
            ELSE
                SET @dataType = N'varbinary(MAX)'

            IF (@isMultiValued = 1)
                IF (@isIndexable = 1)
                    SET @columnName = N'attributeValueBinaryIX';
                ELSE
                    SET @columnName = N'attributeValueBinary';
            ELSE
                SET @columnName = @name;
        END

    ELSE IF (@type = 4) -- Boolean
        BEGIN
            IF (@isIndexed = 1)
                THROW 50000, N'Cannot created an indexed attribute of type boolean', 1;
            ELSE IF (@isIndexable = 1)
                THROW 50001, N'Cannot created an indexable attribute of type boolean', 1;
            ELSE IF (@isMultiValued = 1)
                THROW 50002, N'Cannot created a multivalued attribute of type boolean', 1;
            ELSE
                SET @dataType = 'bit'

            SET @columnName = @name;
        END

    ELSE IF (@type = 99) -- DateTime
        BEGIN
            SET @dataType = 'datetime2(3)';

            IF (@isMultiValued = 1)
                SET @columnName = N'attributeValueDateTime';
            ELSE
                SET @columnName = @name;

            SET @isIndexable = 1;
        END

    ELSE
        THROW 50000, N'Unknown or unsupported data type', 1;
                

    BEGIN TRANSACTION
    
        IF (@tableName = 'MA_Objects')
            BEGIN
                BEGIN TRANSACTION
                    DECLARE @sql nvarchar(4000) = N'ALTER TABLE [dbo].[MA_Objects] ADD ' + @name + N' ' + @dataType + N' NULL';
                    EXEC sp_executesql @sql
                
                    if (@dataType = 4)
                    BEGIN
                        SET @sql = 'ALTER TABLE [dbo].[MA_Objects] ADD CONSTRAINT DF_MA_Objects_' + @name + ' DEFAULT 0 FOR ' + @name
                        EXEC sp_executesql @sql
                    END

                    ALTER TABLE dbo.MA_Objects SET (LOCK_ESCALATION = TABLE)
                COMMIT

                EXEC [dbo].[spSchemaRebuildDeltaView];

                IF (@isIndexed = 1)
                    EXEC [dbo].[spSchemaCreateIndex] @name
            END

        UPDATE dbo.MA_SchemaAttributes 
            SET 
            [dbo].[MA_SchemaAttributes].[IsIndexable] = @isIndexable, 
            [dbo].[MA_SchemaAttributes].[IsIndexed] = @isIndexed,  
            [dbo].[MA_SchemaAttributes].[TableName] = @tableName, 
            [dbo].[MA_SchemaAttributes].[ColumnName] = @columnName 
        WHERE [dbo].[MA_SchemaAttributes].[ID] = @ID;
        
    COMMIT
END

GO

    DECLARE @attributeName nvarchar(50)

    DECLARE attributeCursor CURSOR 
      LOCAL STATIC READ_ONLY FORWARD_ONLY
    FOR 
    SELECT [Name] FROM [dbo].[MA_SchemaAttributes]
    WHERE [IsBuiltIn] = 0 AND [Type] = 4 

    OPEN attributeCursor
    FETCH NEXT FROM attributeCursor INTO @attributeName
    WHILE @@FETCH_STATUS = 0
    BEGIN 

    DECLARE @constraintName nvarchar(50) = 'DF_MA_Objects_' + @attributeName;

    IF OBJECT_ID(@constraintName, 'D') IS NULL
    BEGIN
        DECLARE @sql nvarchar(4000) = 
        'ALTER TABLE [dbo].[MA_Objects] ADD CONSTRAINT ' + @constraintName + ' DEFAULT 0 FOR ' + @attributeName
        EXEC sp_executesql @sql
    END
        FETCH NEXT FROM attributeCursor INTO @attributeName
    END
    CLOSE attributeCursor
    DEALLOCATE attributeCursor

GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 1, 'Boolean_default_update', GETDATE())

