-- =============================================
-- Author:		Ryan Newington
-- Create date: 21-01-2014
-- Description:	Updates the ID of an MAObject
-- =============================================
CREATE PROCEDURE [dbo].[spChangeMAObjectId]
	@oldId uniqueidentifier,
	@newId uniqueidentifier,
	@undelete bit = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF @undelete = 0
		UPDATE [dbo].[MA_Objects]
		SET [dbo].[MA_Objects].[objectId]=@newId 
		WHERE [dbo].[MA_Objects].[objectId]=@oldId
	ELSE
		UPDATE [dbo].[MA_Objects]
		SET [dbo].[MA_Objects].[objectId]=@newId, 
			[dbo].[MA_Objects].[deleted]=0
		WHERE [dbo].[MA_Objects].[objectId]=@oldId
END
