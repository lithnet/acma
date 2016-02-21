-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Obtains all the objects that reference the specified object, along with the name of the referencing attribute
-- =============================================
CREATE PROCEDURE [dbo].[spGetReferences]
	@id uniqueIdentifier
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [objectId], [attributeName] FROM [dbo].[MA_References] WHERE [dbo].[MA_References].[value]=@id
END