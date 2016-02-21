ALTER TRIGGER [dbo].[trigger_modify_MA_References]
ON [dbo].[MA_References]
FOR INSERT, UPDATE AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;

        DECLARE cur CURSOR LOCAL FOR
        SELECT [inserted].[objectId] FROM [inserted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'modify';
            FETCH NEXT FROM cur INTO @objectId
        END

        CLOSE cur
        DEALLOCATE cur
    END
GO
PRINT N'Creating [dbo].[trigger_delete_MA_References]...';


GO

CREATE TRIGGER [dbo].[trigger_delete_MA_References]
ON [dbo].[MA_References]
FOR DELETE AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;

        DECLARE cur CURSOR LOCAL FOR
        SELECT [deleted].[objectId] FROM [deleted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'modify';
            FETCH NEXT FROM cur INTO @objectId
        END

        CLOSE cur
        DEALLOCATE cur
    END
GO
PRINT N'Update complete.';


GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 10, 'Updates to spSchemaDeleteAttribute', GETDATE())
