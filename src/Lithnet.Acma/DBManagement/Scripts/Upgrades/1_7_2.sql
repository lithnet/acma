GO
PRINT N'Altering [dbo].[MA_Objects_Delta]...';

GO

IF NOT EXISTS (
  SELECT * FROM   sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[MA_Objects_Delta]') AND name = 'objectClass')
	BEGIN
		ALTER TABLE [dbo].[MA_Objects_Delta] ADD [objectClass] NVARCHAR (50) NULL;
	END
GO

PRINT N'Altering [dbo].[spCreateDeltaEntry]...';

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 11/1/2014
-- Description:	Creates or updates a record in the delta change table
-- =============================================
ALTER PROCEDURE [dbo].[spCreateDeltaEntry]
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
GO
PRINT N'Altering [dbo].[trigger_delete_MA_Objects]...';

GO
ALTER TRIGGER [dbo].[trigger_delete_MA_Objects]
ON [dbo].[MA_Objects]
FOR DELETE AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;
		DECLARE @objectClass nvarchar(50);

        DECLARE cur CURSOR LOCAL FOR
        SELECT [deleted].[objectId], [deleted].[objectClass] FROM [deleted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId, @objectClass

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'delete', @objectClass;
            FETCH NEXT FROM cur INTO @objectId, @objectClass
        END

        CLOSE cur
        DEALLOCATE cur
    END
GO
PRINT N'Altering [dbo].[trigger_modify_MA_Objects]...';

GO
ALTER TRIGGER [dbo].[trigger_modify_MA_Objects]
ON [dbo].[MA_Objects]
FOR UPDATE AS
BEGIN
    SET NOCOUNT ON
        
    IF ((SELECT TRIGGER_NESTLEVEL()) > 1)
        RETURN;

    DECLARE @objectId uniqueidentifier;
    DECLARE @oldDeleted bigint;
    DECLARE @newDeleted bigint;
	DECLARE @deletedObjectClass nvarchar(50);

    DECLARE cur CURSOR LOCAL FOR
        SELECT [inserted].[objectId], [deleted].[deleted], [inserted].[deleted], [deleted].[objectClass] FROM [inserted]
        INNER JOIN [deleted] ON [inserted].[objectId] = [deleted].[objectId];

    OPEN cur
    FETCH NEXT FROM cur into @objectId, @oldDeleted, @newDeleted, @deletedObjectClass

    WHILE @@FETCH_STATUS = 0 
    BEGIN
        DECLARE @changeType nvarchar(10) =
            CASE
                WHEN (@oldDeleted >= 0 AND @newDeleted > 0) THEN N'delete'
                WHEN (@oldDeleted > 0 AND @newDeleted = 0) THEN N'add'
                ELSE N'modify'
            END;

        EXEC [dbo].[spCreateDeltaEntry] @objectId, @changeType, @deletedObjectClass;
        FETCH NEXT FROM cur INTO  @objectId, @oldDeleted, @newDeleted, @deletedObjectClass
    END

    CLOSE cur
    DEALLOCATE cur
END

GO
-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the database objects from the delta table up to the specified watermark
-- =============================================
ALTER PROCEDURE [dbo].[spGetMAObjectsDelta]
@watermark rowversion = NULL,
@deleted bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF (@deleted = 0)
        IF (@watermark IS NULL)
            SELECT	   o.*, 
                       d.[operation], 
                       d.[rowversion] as [rowversiondelta],
                       d.[objectId] as [deltaObjectId],
					   d.[objectClass] as [deltaObjectClass]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
            WHERE	   o.[deleted] = 0

        ELSE
            SELECT	   o.*, 
                       d.[operation], 
                       d.[rowversion] as [rowversiondelta],
                       d.[objectId] as [deltaObjectId],
					   d.[objectClass] as [deltaObjectClass]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
            WHERE	   o.[deleted] = 0
            AND		   d.[rowversion] <= @watermark

    ELSE
        IF (@watermark IS NULL)
            SELECT	   o.*, 
                       d.[operation], 
                       d.[rowversion] as [rowversiondelta],
                       d.[objectId] as [deltaObjectId],
					   d.[objectClass] as [deltaObjectClass]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
        ELSE
            SELECT	   o.*, 
                       d.[operation], 
                       d.[rowversion] as [rowversiondelta],
                       d.[objectId] as [deltaObjectId],
					   d.[objectClass] as [deltaObjectClass]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
            WHERE	   d.[rowversion] <= @watermark
END
GO
PRINT N'Refreshing [dbo].[spChangeMAObjectId]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spChangeMAObjectId]';


GO
PRINT N'Refreshing [dbo].[spCreateMAObject]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spCreateMAObject]';


GO
PRINT N'Refreshing [dbo].[spDeleteMAObject]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spDeleteMAObject]';


GO
PRINT N'Refreshing [dbo].[spGetHighWatermarkMAObjects]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spGetHighWatermarkMAObjects]';


GO
PRINT N'Refreshing [dbo].[spGetMAObject]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spGetMAObject]';


GO
PRINT N'Refreshing [dbo].[spGetMAObjects]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spGetMAObjects]';


GO
PRINT N'Refreshing [dbo].[spGetMAObjectsDelta]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spGetMAObjectsDelta]';


GO
PRINT N'Refreshing [dbo].[spClearMAObjectsDelta]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spClearMAObjectsDelta]';


GO
PRINT N'Refreshing [dbo].[spGetHighWatermarkMAObjectsDelta]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spGetHighWatermarkMAObjectsDelta]';


GO
PRINT N'Update complete.';

GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 7, 2, 'Fixes to version 1.7.0 and 1.7.1 inconsistencies', GETDATE())
