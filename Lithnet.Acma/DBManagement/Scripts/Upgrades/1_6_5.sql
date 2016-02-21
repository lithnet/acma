ALTER DATABASE [$(DatabaseName)]
    SET ARITHABORT ON, 
    CONCAT_NULL_YIELDS_NULL ON, 
    QUOTED_IDENTIFIER ON, 
    ANSI_NULLS ON, 
    ANSI_PADDING ON,
    ANSI_WARNINGS ON
    SET NUMERIC_ROUNDABORT OFF;

    
INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 5, 'Set DB defaults', GETDATE())
