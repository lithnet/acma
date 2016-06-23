-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Determines if a column exists in the MA_Objects table
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaHasColumn]
	@columnName nvarchar(50) 
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MA_Objects' AND COLUMN_NAME=@columnName)
		BEGIN
			SELECT 1;
			RETURN;
		END
	ELSE
		BEGIN
			SELECT 0;
			RETURN;
		END
END
