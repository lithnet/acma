   
-- Drop unused stored procedures
IF (OBJECT_ID('spClearDeltaObject' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spClearDeltaObject];

PRINT N'Dropping [dbo].[spDoesAttributeExist]...';

IF (OBJECT_ID('spDoesAttributeExist' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeExist];

PRINT N'Dropping [dbo].[spDoesAttributeValueExistBinary]...';

IF (OBJECT_ID('spDoesAttributeValueExistBinary' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeValueExistBinary];

PRINT N'Dropping [dbo].[spDoesAttributeValueExistBoolean]...';

IF (OBJECT_ID('spDoesAttributeValueExistBoolean' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeValueExistBoolean];

PRINT N'Dropping [dbo].[spDoesAttributeValueExistDateTime]...';

IF (OBJECT_ID('spDoesAttributeValueExistDateTime' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeValueExistDateTime];

PRINT N'Dropping [dbo].[spDoesAttributeValueExistInt]...';

IF (OBJECT_ID('spDoesAttributeValueExistInt' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeValueExistInt];

PRINT N'Dropping [dbo].[spDoesAttributeValueExistReference]...';

IF (OBJECT_ID('spDoesAttributeValueExistReference' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeValueExistReference];

PRINT N'Dropping [dbo].[spDoesAttributeValueExistString]...';

IF (OBJECT_ID('spDoesAttributeValueExistString' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spDoesAttributeValueExistString];

PRINT N'Dropping [dbo].[spGetMAObjectMVAttributes]...';

IF (OBJECT_ID('spGetMAObjectMVAttributes' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spGetMAObjectMVAttributes];

PRINT N'Dropping [dbo].[spSchemaDoesMVAttributeExist]...';

IF (OBJECT_ID('spSchemaDoesMVAttributeExist' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spSchemaDoesMVAttributeExist];

PRINT N'Dropping [dbo].[spSchemaGetAttributeColumnName]...';

IF (OBJECT_ID('spSchemaGetAttributeColumnName' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spSchemaGetAttributeColumnName];

PRINT N'Dropping [dbo].[spSchemaGetAttributeTableName]...';

IF (OBJECT_ID('spSchemaGetAttributeTableName' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spSchemaGetAttributeTableName];

PRINT N'Dropping [dbo].[spSchemaGetColumns]...';

IF (OBJECT_ID('spSchemaGetColumns' ,'P') IS NOT NULL)
    DROP PROCEDURE [dbo].[spSchemaGetColumns];

PRINT N'Altering [dbo].[spClearMAObjectsDelta]...';

-- Update stored procedures
GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Clears the delta table from the specified watermark
-- =============================================
ALTER PROCEDURE [dbo].[spClearMAObjectsDelta]
@watermark rowversion
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[MA_Objects_Delta] WHERE [dbo].[MA_Objects_Delta].[rowversion] <= @watermark
END
GO

PRINT N'Altering [dbo].[spCreateMAObject]...';
GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Creates a new object of the specified class
-- =============================================
ALTER PROCEDURE [dbo].[spCreateMAObject]
@id uniqueidentifier,
@objectClass nvarchar(50),
@shadowParent uniqueidentifier = null
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[MA_Objects] ([dbo].[MA_Objects].[objectId], [dbo].[MA_Objects].[objectClass], [dbo].[MA_Objects].[shadowParent]) VALUES (@id, @objectClass, @shadowParent);

    SELECT TOP 1 * FROM [dbo].[MA_Objects]
    WHERE [dbo].[MA_Objects].[objectId] = @id;
END
GO
PRINT N'Altering [dbo].[spGetHighWatermarkMAObjects]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the high watermark from the MA_Objects table
-- =============================================
ALTER PROCEDURE [dbo].[spGetHighWatermarkMAObjects]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MAX([dbo].[MA_Objects].[rowversion]) FROM [dbo].[MA_Objects]
END
GO
PRINT N'Altering [dbo].[spGetHighWatermarkMAObjectsDelta]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the highest value for the rowversion
--				column of the MA_Objects_Delta table				
-- =============================================
ALTER PROCEDURE [dbo].[spGetHighWatermarkMAObjectsDelta]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MAX([dbo].[MA_Objects_Delta].[rowversion]) FROM [dbo].[MA_Objects_Delta]
END
GO
PRINT N'Altering [dbo].[spGetMAObject]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets a specific object from the database
-- =============================================
ALTER PROCEDURE [dbo].[spGetMAObject]
    @id uniqueIdentifier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[objectId]=@id
END
GO
PRINT N'Altering [dbo].[spGetMAObjects]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets database objects up to the specified watermark value
-- =============================================
ALTER PROCEDURE [dbo].[spGetMAObjects]
@watermark rowversion = NULL,
@deleted bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF (@deleted = 0)
        IF (@watermark IS NULL)
            SELECT * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[deleted] = 0
        ELSE
            SELECT * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[deleted] = 0 AND [dbo].[MA_Objects].[rowversion] <= @watermark
    ELSE
        IF (@watermark is NULL)
            SELECT * FROM [dbo].[MA_Objects] 
        ELSE
            SELECT * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[rowversion] <= @watermark
        
END
GO
PRINT N'Altering [dbo].[spGetMAObjectsDelta]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the database objects from the delta table up to the specified watermark
-- =============================================
ALTER PROCEDURE [dbo].[spGetMAObjectsDelta]
@watermark rowversion = NULL,
@deleted bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF (@deleted = 0)
        IF (@watermark IS NULL)
            SELECT * FROM [dbo].[v_MA_Objects_Delta] WHERE [dbo].[v_MA_Objects_Delta].[deleted] = 0
        ELSE
            SELECT * FROM [dbo].[v_MA_Objects_Delta] WHERE [dbo].[v_MA_Objects_Delta].[deleted] = 0 AND [dbo].[v_MA_Objects_Delta].[rowversiondelta] <= @watermark
    ELSE
        IF (@watermark IS NULL)
            SELECT * FROM [dbo].[v_MA_Objects_Delta] 
        ELSE
            SELECT * FROM [dbo].[v_MA_Objects_Delta] WHERE [dbo].[v_MA_Objects_Delta].[rowversiondelta] <= @watermark
        
END
GO
PRINT N'Altering [dbo].[spGetObjectsOfClass]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 2/09/2014
-- Description:	Gets all objects of a specified object class
-- =============================================
ALTER PROCEDURE [dbo].[spGetObjectsOfClass]
    @objectClass nvarchar(50)
AS
BEGIN
    DECLARE @id INT
    DECLARE @name NVARCHAR(100)
    DECLARE @getid CURSOR
    DECLARE @columns NVARCHAR(MAX)
    SET @columns = 'id'

    SET @getid = CURSOR FOR
        SELECT 
            a.[ColumnName]
        FROM
             [dbo].[MA_SchemaMapping] m
        JOIN
             [dbo].[MA_SchemaObjectClasses] o
        ON 
            m.[ObjectClassID] = o.[ID]
        JOIN 
            [dbo].[MA_SchemaAttributes] a
        ON 
            a.[ID] = m.[AttributeID]
        WHERE 
            o.[Name] = @objectClass
            AND m.[InheritanceSourceAttributeID] is null
            AND a.[IsMultivalued] = 0
            AND a.[Type] != 2

    OPEN @getid
    FETCH NEXT FROM @getid INTO @name
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @columns = @columns + ', ' + QUOTENAME(@name)
        FETCH NEXT FROM @getid INTO @name
    END

    CLOSE @getid
    DEALLOCATE @getid

    DECLARE @params nvarchar(200) = N'@objectClass nvarchar(50)'
    DECLARE @sql nvarchar(max) = N'SELECT ' + @columns + N' FROM MA_Objects where objectClass=@objectClass'
    EXEC sp_executesql @sql, @params, @objectClass
END
GO
PRINT N'Altering [dbo].[spSchemaDeleteIndex]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Deletes an index from the MA_Objects table
-- =============================================
ALTER PROCEDURE [dbo].[spSchemaDeleteIndex]
    @attributeName nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sql nvarchar(4000);
    
    DECLARE @table nvarchar(50);

    SELECT @table = [MA_SchemaAttributes].[TableName]
    FROM [dbo].[MA_SchemaAttributes] WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName;
    
    IF (@table != 'MA_Objects')
        THROW 50010, N'Cannot delete the index of an attribute not in the objects table', 1;

    BEGIN TRANSACTION
        IF EXISTS (SELECT [sys].[indexes].[name] FROM sys.indexes WHERE [sys].[indexes].[name] = N'IX_MA_Objects_' + @attributeName)
            BEGIN
                SET @sql = N'DROP INDEX [IX_MA_Objects_' + @attributeName +'] ON [dbo].[MA_Objects]'
                EXEC sp_executesql @sql
            END
                    
        UPDATE dbo.MA_SchemaAttributes
        SET [dbo].[MA_SchemaAttributes].[IsIndexed] = 0
        WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName
    
    COMMIT TRANSACTION
END
GO
PRINT N'Altering [dbo].[spSequenceCreate]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2014
-- Description:	Creates a new sequence in the database
-- =============================================
ALTER PROCEDURE [dbo].[spSequenceCreate]
    @sequenceName nvarchar(128),
    @startsWith bigint,
    @incrementBy bigint,
    @minValue bigint,
    @maxValue bigint,
    @cycle bit

AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql nvarchar(max);

    set @sql =N'CREATE SEQUENCE [dbo].' + quotename(@sequenceName) + N' 
    AS bigint
    START WITH ' + CAST(@startsWith as nvarchar(50))  + N'
    INCREMENT BY ' + CAST(@incrementBy as nvarchar(50)) 
    
    IF (@minValue is null)
        SET @sql = @SQL + N' NO MINVALUE'
    ELSE
        SET @sql = @sql + N' MINVALUE ' + CAST(@minValue as nvarchar(50)) 

    IF (@maxValue is null)
        SET @sql = @sql + N' NO MAXVALUE'
    ELSE
        SET @sql = @sql + N' MAXVALUE ' + CAST(@maxValue as nvarchar(50)) 

    IF (@cycle = 1)
        SET @sql = @sql + N' CYCLE'
    ELSE
        SET @sql = @sql + N' NO CYCLE'


    EXECUTE sp_executesql @sql;

END
GO
PRINT N'Altering [dbo].[spSequenceDelete]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2014
-- Description:	Deletes a sequence from the database
-- =============================================
ALTER PROCEDURE [dbo].[spSequenceDelete]
    @sequenceName nvarchar(128)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sql nvarchar(500) = N'DROP SEQUENCE [dbo].' + quotename(@sequenceName);
    EXECUTE sp_executesql @sql;
END
GO
PRINT N'Altering [dbo].[spSequenceGet]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2015
-- Description:	Gets a sequence definition from the database
-- =============================================
ALTER PROCEDURE [dbo].[spSequenceGet]
AS
BEGIN
    SET NOCOUNT ON;

   SELECT
        seq.name AS [Sequence Name],
        CAST(seq.precision AS int) AS [NumericPrecision],
        ISNULL(seq.start_value,N'''') AS [StartValue],
        ISNULL(seq.increment,N'''') AS [IncrementValue],
        ISNULL(seq.minimum_value,N'''') AS [MinValue],
        ISNULL(seq.maximum_value,N'''') AS [MaxValue],
        CAST(seq.is_cycling AS bit) AS [IsCycleEnabled],
        ISNULL(seq.cache_size,0) AS [CacheSize],
        ISNULL(seq.current_value,N'''') AS [CurrentValue]
    FROM
        [sys].sequences AS seq

END
GO
PRINT N'Altering [dbo].[spSequenceGetNextValue]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2015
-- Description:	Gets the next value from the specified sequence
-- =============================================
ALTER PROCEDURE [dbo].[spSequenceGetNextValue]
    @sequenceName nvarchar(50)
AS
    SET NOCOUNT ON;
    DECLARE @nextValue bigint;

    DECLARE @sql nvarchar(1000) = N'SET @nextValueOUT = NEXT VALUE FOR [dbo].' + QUOTENAME(@sequenceName);
    DECLARE @params nvarchar(100) = N'@nextValueOUT bigint OUTPUT';
    EXEC sp_executesql @sql, @params, @nextValueOUT = @nextValue OUTPUT;

    return @nextValue;
GO
PRINT N'Altering [dbo].[spSequenceModify]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2015
-- Description:	Modifies the parameters of a database sequence
-- =============================================
ALTER PROCEDURE [dbo].[spSequenceModify]
    @sequenceName nvarchar(128),
    @newSequenceName nvarchar(128),
    @restartWith bigint,
    @incrementBy bigint,
    @minValue bigint,
    @maxValue bigint,
    @cycle bit
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sql nvarchar(500);

    set @sql =N'ALTER SEQUENCE [dbo].' + quotename(@sequenceName) + N' 
    RESTART WITH ' + CAST(@restartWith as nvarchar(50))  + N'
    INCREMENT BY ' + CAST(@incrementBy as nvarchar(50)) 
    
    IF (@minValue is null)
        SET @sql = @SQL + N' NO MINVALUE'
    ELSE
        SET @sql = @sql + N' MINVALUE ' + CAST(@minValue as nvarchar(50)) 

    IF (@maxValue is null)
        SET @sql = @sql + N' NO MAXVALUE'
    ELSE
        SET @sql = @sql + N' MAXVALUE ' + CAST(@maxValue as nvarchar(50)) 

    IF (@cycle = 1)
        SET @sql = @sql + N' CYCLE'
    ELSE
        SET @sql = @sql + N' NO CYCLE'

    EXECUTE sp_executesql @sql;

    IF (@newSequenceName is not null)
        IF (@newSequenceName != @sequenceName)
            BEGIN
                DECLARE @existingName nvarchar(150) = '[dbo].' + quotename(@sequenceName);
                EXECUTE sp_rename @existingName, @newSequenceName;
            END

END
GO
PRINT N'Creating [dbo].[spGetReferences]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Obtains all the objects that reference the specified object, along with the name of the referencing attribute
-- =============================================
CREATE PROCEDURE [dbo].[spGetReferences]
    @id uniqueIdentifier
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [ObjectId], [AttributeName] FROM [dbo].[MA_References] WHERE [dbo].[MA_References].[Value]=@id
END
GO
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


    SELECT 
        @isIndexed = [dbo].[MA_SchemaAttributes].[IsIndexed],
        @tableName = [dbo].[MA_SchemaAttributes].[TableName]
    FROM [dbo].[MA_SchemaAttributes] WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName;

    BEGIN TRANSACTION
        IF (@tableName = 'MA_Objects')
            BEGIN
                IF (@isIndexed = 1)
                    EXEC [dbo].spSchemaDeleteIndex @attributeName;

                DECLARE @columnName nvarchar(150);
                SET @columnName = 'MA_Objects.' + @attributeName;
                EXEC [dbo].[sp_rename] @columnName, @newAttributeName, 'COLUMN';
                EXEC [dbo].[spSchemaRebuildDeltaView];
            
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
PRINT N'Altering [dbo].[spSchemaSetupNewAttribute]...';

GO
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
                DECLARE @sql nvarchar(4000) = N'ALTER TABLE [dbo].[MA_Objects] ADD ' + @name + N' ' + @dataType + N' NULL';
                EXEC sp_executesql @sql
                ALTER TABLE dbo.MA_Objects SET (LOCK_ESCALATION = TABLE)
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

    SELECT TOP 1
        @count = 1,
        @isBuiltIn = [dbo].[MA_SchemaAttributes].[IsBuiltIn],
        @name = [dbo].[MA_SchemaAttributes].[Name],
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

                DECLARE @hasColumn bit;
                EXEC @hasColumn = [dbo].[spSchemaHasColumn] @columnName;

                if (@hasColumn = 1)
                    BEGIN
                        SET @sql = N'ALTER TABLE [dbo].[MA_Objects] DROP COLUMN ' + @columnName;
                        EXEC sp_executesql @sql;
                        ALTER TABLE [dbo].[MA_Objects] SET (LOCK_ESCALATION = TABLE);
                    END

                EXEC [dbo].[spSchemaRebuildDeltaView];
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
PRINT N'Checking existing data against newly created constraints';

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'MA_References'))
BEGIN
    CREATE TABLE [dbo].[MA_References] (
        [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
        [objectId]      UNIQUEIDENTIFIER NOT NULL,
        [attributeName] NVARCHAR (50)    NOT NULL,
        [value]         UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT [PK_MA_References] PRIMARY KEY CLUSTERED ([ID] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_MA_References_attributeName_value]
        ON [dbo].[MA_References]([attributeName] ASC, [value] ASC);

    CREATE NONCLUSTERED INDEX [IX_MA_References_value]
        ON [dbo].[MA_References]([value] ASC);

    CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_References_objectID_attributeName]
        ON [dbo].[MA_References]([objectId] ASC, [attributeName] ASC, [value] ASC);

    ALTER TABLE [dbo].[MA_References] WITH NOCHECK
        ADD CONSTRAINT [FK_MA_References_MA_Objects] FOREIGN KEY ([objectId]) REFERENCES [dbo].[MA_Objects] ([objectId]) ON DELETE CASCADE ON UPDATE CASCADE;

    ALTER TABLE [dbo].[MA_References] WITH NOCHECK
        ADD CONSTRAINT [FK_MA_References_MA_SchemaAttributes] FOREIGN KEY ([attributeName]) REFERENCES [dbo].[MA_SchemaAttributes] ([Name]) ON DELETE CASCADE ON UPDATE CASCADE;
END

GO

    CREATE TRIGGER [dbo].[trigger_modify_MA_References]
    ON [dbo].[MA_References]
    FOR INSERT, DELETE, UPDATE AS
        SET NOCOUNT ON
        INSERT INTO [dbo].[MA_Objects_Delta]
            SELECT DISTINCT inserted.[objectId], 'modify', null
            FROM inserted LEFT OUTER JOIN [dbo].[MA_Objects_Delta] ON
                inserted.[objectId] = [dbo].[MA_Objects_Delta].[objectId]
            WHERE
                ([dbo].[MA_Objects_Delta].[objectId] IS NULL)
        
GO

ALTER TABLE [dbo].[MA_References] WITH CHECK CHECK CONSTRAINT [FK_MA_References_MA_Objects];

ALTER TABLE [dbo].[MA_References] WITH CHECK CHECK CONSTRAINT [FK_MA_References_MA_SchemaAttributes];

GO
PRINT N'Schema update complete';

GO

PRINT N'Performing data migration';
GO

    DECLARE @attributeName nvarchar(50)
    DECLARE @isMultiValued bit

    DECLARE attributeCursor CURSOR 
      LOCAL STATIC READ_ONLY FORWARD_ONLY
    FOR 
    SELECT [Name], [IsMultivalued] FROM [dbo].[MA_SchemaAttributes]
    WHERE [IsBuiltIn] = 0 AND [Type] = 2 AND [IsMultivalued] = 0

    OPEN attributeCursor
    FETCH NEXT FROM attributeCursor INTO @attributeName, @isMultivalued
    WHILE @@FETCH_STATUS = 0
    BEGIN 
        IF @isMultiValued = 0
            BEGIN
                declare @sql nvarchar(4000) = '
                INSERT INTO [dbo].[MA_References] (ObjectId, AttributeName, Value)
                SELECT [objectId], ''' + @attributeName + ''' as attributeName, [' + @attributeName + ']
                FROM [dbo].[MA_Objects]
                WHERE [' + @attributeName + '] IS NOT NULL'

                EXEC sp_sqlexec @sql
    
                SET @sql ='
                DROP INDEX [dbo].[MA_Objects].[IX_MA_Objects_' + @attributeName + ']'
                EXEC sp_sqlexec @sql
            
                SET @sql ='
                ALTER TABLE [dbo].[MA_Objects]
                    DROP COLUMN [' + @attributeName + ']'

                EXEC sp_sqlexec @sql
            END
        ELSE
            BEGIN
                INSERT INTO [dbo].[MA_References] (ObjectID, AttributeName, Value)
                SELECT [objectID], [attributeName], [attributeValueReference]
                FROM [dbo].[MA_Attributes]
                WHERE [attributeName] = @attributeName

                DELETE FROM [dbo].[MA_Attributes]
                WHERE [attributeName] = @attributeName
            END

        FETCH NEXT FROM attributeCursor INTO @attributeName, @isMultivalued
    END
    CLOSE attributeCursor
    DEALLOCATE attributeCursor

GO

PRINT N'Dropping old schema columns';
DROP INDEX [dbo].[MA_Attributes].[IX_MA_Attributes_attributeName_attributeValueReference]

ALTER TABLE [dbo].[MA_Attributes]
    DROP COLUMN [attributeValueReference]


UPDATE [dbo].[MA_SchemaAttributes]
SET [TableName] = 'MA_References', [ColumnName] = 'Value'
WHERE [IsBuiltIn] = 0 AND [Type] = 2
GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 0, 'Reference_Update', GETDATE())


EXEC [dbo].[spSchemaRebuildDeltaView];