-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Deletes an index from the MA_Objects table
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaDeleteIndex]
	-- Add the parameters for the stored procedure here
	@attributeName nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @sql nvarchar(4000);
	
	DECLARE @isMultiValued bit;

	SELECT @isMultiValued = IsMultiValued 
	FROM dbo.MA_SchemaAttributes WHERE Name = @attributeName;
	
	IF (@isMultiValued = 1)
		THROW 50010, N'Cannot delete the index of a multivalued attribute', 1;
		
	BEGIN TRANSACTION
		IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_MA_Objects_' + @attributeName)
			BEGIN
				SET @sql = N'DROP INDEX [IX_MA_Objects_' + @attributeName +'] ON [dbo].[MA_Objects]'
				EXEC sp_executesql @sql
			END
					
		UPDATE dbo.MA_SchemaAttributes
		SET IsIndexed = 0
		WHERE Name = @attributeName
	
	COMMIT TRANSACTION
END
