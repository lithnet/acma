-- =============================================
-- Author:		Ryan Newington
-- Create date: 21-01-2014
-- Description:	Updates the ID of an MAObject
-- =============================================
ALTER PROCEDURE [dbo].[spChangeMAObjectId]
	@oldId uniqueidentifier,
	@newId uniqueidentifier,
	@undelete bit = 0
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION
		IF @undelete = 0
			BEGIN
				UPDATE [dbo].[MA_Objects]
					SET 
						[dbo].[MA_Objects].[objectId]=@newId 
					WHERE 
						[dbo].[MA_Objects].[objectId]=@oldId

				UPDATE [dbo].[MA_Objects]
					SET 
						[dbo].[MA_Objects].[shadowParent]=@newId
					WHERE 
						[dbo].[MA_Objects].[shadowParent]=@oldId
			END
		ELSE
			BEGIN
				UPDATE [dbo].[MA_Objects]
					SET 
						[dbo].[MA_Objects].[objectId]=@newId, 
						[dbo].[MA_Objects].[deleted]=0
					WHERE 
						[dbo].[MA_Objects].[objectId]=@oldId

				UPDATE [dbo].[MA_Objects]
					SET 
						[dbo].[MA_Objects].[shadowParent]=@newId
					WHERE 
						[dbo].[MA_Objects].[shadowParent]=@oldId
			END
	COMMIT
END

GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 11, 'Updates to [spChangeMAObjectId]', GETDATE())
