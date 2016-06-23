PRINT N'Altering [dbo].[spDeleteMAObject]...';
GO

-- =============================================
-- Author:		Ryan Newington
-- Create date: 
-- Description:	Deletes an MAObject from the database
-- =============================================
ALTER PROCEDURE [dbo].[spDeleteMAObject]
@id uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[objectId]=@id;
	
END

GO
PRINT N'Update complete.';

GO

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 7, 1, 'Updates to spDeleteMAObject', GETDATE())
