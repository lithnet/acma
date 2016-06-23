-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Gets the name of the table for the specified atttribute type
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaGetAttributeTableName]
	@isMultiValued bit,
	@type int,
	@operation int,
	@indexable bit,
	@indexed bit,
	@tableName nvarchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
		
	IF (@isMultiValued = 1)
		SET @tableName = 'MA_Attributes';
	ELSE
		SET @tableName = 'MA_Objects';
END
