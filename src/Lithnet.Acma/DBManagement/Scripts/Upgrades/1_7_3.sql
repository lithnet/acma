GO
PRINT N'Allowing snapshot isolation...';

GO
-- The resource service reads a paged result set inside one snapshot transaction
ALTER DATABASE [$(DatabaseName)] SET ALLOW_SNAPSHOT_ISOLATION ON;

GO
PRINT N'Update complete.';

GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 7, 3, 'Allows snapshot isolation', GETDATE())
