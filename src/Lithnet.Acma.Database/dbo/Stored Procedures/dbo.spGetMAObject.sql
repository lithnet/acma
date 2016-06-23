-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets a specific object from the database
-- =============================================
CREATE PROCEDURE [dbo].[spGetMAObject]
	@id uniqueIdentifier
AS
BEGIN
	SET NOCOUNT ON;
	SELECT TOP 1 * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[objectId]=@id
END
