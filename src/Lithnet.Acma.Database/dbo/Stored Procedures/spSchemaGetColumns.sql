-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Gets the custom columns defined in the table schema
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaGetColumns]
AS
BEGIN
	SET NOCOUNT ON;

    SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE 
	TABLE_NAME = 'MA_Objects'
	AND COLUMN_NAME != 'id'
	AND COLUMN_NAME != 'objectId'
	AND COLUMN_NAME != 'rowversion'
	AND COLUMN_NAME != 'inheritedUpdate'
	AND COLUMN_NAME != 'deleted'
	AND COLUMN_NAME != 'objectClass'
	AND COLUMN_NAME != 'shadowParent'
END
