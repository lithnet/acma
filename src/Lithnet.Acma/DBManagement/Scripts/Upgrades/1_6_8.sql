ALTER TRIGGER [dbo].[trigger_modify_MA_Attributes]
ON [dbo].[MA_Attributes]
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

CREATE TRIGGER [dbo].[trigger_delete_MA_Attributes]
ON [dbo].[MA_Attributes]
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

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 8, 'Fixed triggers on MA_Attributes table', GETDATE())
