-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Deletes an index from the MA_Objects table
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaDeleteIndex]
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
