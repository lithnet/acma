-- =============================================
-- Author:		Ryan Newington
-- Create date: 11/1/2014
-- Description:	Creates or updates a record in the delta change table
-- =============================================
CREATE PROCEDURE [dbo].[spCreateDeltaEntry]
	@objectId uniqueidentifier,
	@changeType nvarchar(10),
	@objectClass nvarchar(50) = null
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @existingChangeType nvarchar(10);
    
	SET @existingChangeType = (SELECT TOP 1 [delta].[operation] FROM [dbo].[MA_Objects_Delta] as [delta]
	WHERE [delta].[objectId] = @objectId);

	SET @changeType = (
		SELECT 
			CASE
				WHEN (@existingChangeType IS NULL) THEN @changeType
				WHEN (@changeType = 'add' AND @existingChangeType = 'add') THEN 'add'
				WHEN (@changeType = 'add' AND @existingChangeType = 'modify') THEN 'modify'
				WHEN (@changeType = 'add' AND @existingChangeType = 'delete') THEN 'modify'
				WHEN (@changeType = 'modify' AND @existingChangeType = 'add') THEN 'add'
				WHEN (@changeType = 'modify' AND @existingChangeType = 'modify') THEN 'modify'
				WHEN (@changeType = 'modify' AND @existingChangeType = 'delete') THEN 'delete'
				WHEN (@changeType = 'delete' AND @existingChangeType = 'add') THEN NULL
				WHEN (@changeType = 'delete' AND @existingChangeType = 'modify') THEN 'delete'
				WHEN (@changeType = 'delete' AND @existingChangeType = 'delete') THEN 'delete'
				ELSE 'modify'
			END
			);

	IF (@changeType IS NULL AND @existingChangeType IS NOT NULL) 
		BEGIN
			DELETE FROM [dbo].[MA_Objects_Delta] WHERE [dbo].[MA_Objects_Delta].[objectId] = @objectId;
			RETURN;
		END

	IF (@changeType IS NULL AND @existingChangeType IS NULL)
		RETURN;

	IF (@existingChangeType IS NULL)
		INSERT INTO [dbo].[MA_Objects_Delta]
			([dbo].[MA_Objects_Delta].[objectId], [dbo].[MA_Objects_Delta].[operation], [dbo].[MA_Objects_Delta].[objectClass])
		VALUES 
			(@objectId, @changeType, @objectClass);
	ELSE
		UPDATE [dbo].[MA_Objects_Delta]
		SET 
		[dbo].[MA_Objects_Delta].[operation]=@changeType,
		[dbo].[MA_Objects_Delta].[objectClass]=@objectClass
		WHERE
		([dbo].[MA_Objects_Delta].[objectId] = @objectId);
END
