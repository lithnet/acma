-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Gets the name of the table for the specified atttribute type
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaGetAttributeColumnName]
	@name nvarchar(50),
	@isMultiValued bit,
	@type int,
	@operation int,
	@isIndexable bit,
	@isIndexed bit,
	@columnName nvarchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
		
	IF (@type = 0) -- String
		BEGIN
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
			IF (@isMultiValued = 1)
				SET @columnName = N'attributeValueInt';
			ELSE
				SET @columnName = @name;

			SET @isIndexable = 1;
		END

	ELSE IF (@type = 2) -- Reference
		BEGIN
			IF (@isMultiValued = 1)
				SET @columnName = N'attributeValueReference';
			ELSE
				SET @columnName = @name;
		END

	ELSE IF (@type = 3) -- Binary
		BEGIN 
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

			SET @columnName = @name;
		END

	ELSE
		THROW 50000, N'Unknown or unsupported data type', 1;
			
END
