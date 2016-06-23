-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Configures a new attribute in the schema
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaSetupNewAttribute]
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
	*/

	DECLARE @dataType nvarchar(30);
	DECLARE @tableName nvarchar(30);
	DECLARE @columnName nvarchar(30);

	DECLARE @operation int;
	DECLARE @isMultiValued bit;
	DECLARE @type int;
	DECLARE @isIndexed bit;
	DECLARE @isIndexable bit;
	DECLARE @name nvarchar(50);
	DECLARE @count int;
	
 	SELECT TOP 1
		@count = 1,
		@name = name,
		@operation = [Operation],
		@isMultiValued = [IsMultiValued],
		@type = [Type],
		@isIndexable = [IsIndexable],
		@isIndexed = [IsIndexed]
	FROM [dbo].[MA_SchemaAttributes]
	WHERE [ID] = @ID

	IF (@count is null)
		THROW 50009, N'Attribute not found', 1;
 
	IF (@operation = 4) -- AcmaInternalTemp
		BEGIN
			SET @isIndexable = 0;
			SET @isIndexed = 0;
		END
	
	IF (@isMultiValued = 1)
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

			IF (@isMultiValued = 1)
				SET @columnName = N'attributeValueReference';
			ELSE
				SET @columnName = @name;
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
				THROW 50002, N'Cannot created a mutlivalued attriibute of type boolean', 1;
			ELSE
				SET @dataType = 'bit'

			SET @columnName = @name;
		END

	ELSE
		THROW 50000, N'Unknown or unsupported data type', 1;
				

	BEGIN TRANSACTION
	
		IF (@isMultiValued = 0)
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
			[IsIndexable] = @isIndexable, 
			[IsIndexed] = @isIndexed,  
			[TableName] = @tableName, 
			[ColumnName] = @columnName 
		WHERE [ID] = @ID;
		
	COMMIT
END
