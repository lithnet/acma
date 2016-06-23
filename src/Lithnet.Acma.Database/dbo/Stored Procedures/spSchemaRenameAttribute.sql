-- =============================================
-- Author:		Ryan Newington
-- Create date: 30-05-2014
-- Description:	Renames an attribute in the schema
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaRenameAttribute]
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

	SELECT @isMultiValued = IsMultiValued 
	FROM dbo.MA_SchemaAttributes WHERE Name = @attributeName;
	
	SELECT @isIndexed = IsIndexed 
	FROM dbo.MA_SchemaAttributes WHERE Name = @attributeName;

	BEGIN TRANSACTION
		IF (@isMultiValued = 0)
			BEGIN
				IF (@isIndexed = 1)
					EXEC [dbo].spSchemaDeleteIndex @attributeName;

				DECLARE @columnName nvarchar(150);
				SET @columnName = 'MA_Objects.' + @attributeName;
				EXEC sp_rename @columnName, @newAttributeName, 'COLUMN';
				EXEC [dbo].[spSchemaRebuildDeltaView];
			
				IF (@isIndexed = 1)
					EXEC [dbo].spSchemaCreateIndex @newAttributeName;
			END

		UPDATE dbo.MA_SchemaAttributes
		SET name = @newAttributeName
		WHERE name = @attributeName

	COMMIT TRANSACTION
END
