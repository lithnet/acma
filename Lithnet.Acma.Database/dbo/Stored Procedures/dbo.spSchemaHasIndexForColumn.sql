-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Determines if an index exists on the MA_Objects table
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaHasIndexForColumn]
	@columnName nvarchar(50)
AS

BEGIN
	IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MA_Objects_' + @columnName)
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
