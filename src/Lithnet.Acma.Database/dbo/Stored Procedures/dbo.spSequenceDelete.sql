-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2014
-- Description:	Deletes a sequence from the database
-- =============================================
CREATE PROCEDURE [dbo].[spSequenceDelete]
	@sequenceName nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @sql nvarchar(500) = N'DROP SEQUENCE [dbo].' + quotename(@sequenceName);
	EXECUTE sp_executesql @sql;
END
