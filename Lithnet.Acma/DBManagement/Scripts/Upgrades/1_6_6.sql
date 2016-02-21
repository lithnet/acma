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
                       d.[objectId] as [deltaObjectId]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
            WHERE	   o.[deleted] = 0

        ELSE
            SELECT	   o.*, 
                       d.[operation], 
                       d.[rowversion] as [rowversiondelta],
                       d.[objectId] as [deltaObjectId]
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
                       d.[objectId] as [deltaObjectId]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
        ELSE
            SELECT	   o.*, 
                       d.[operation], 
                       d.[rowversion] as [rowversiondelta],
                       d.[objectId] as [deltaObjectId]
            FROM       [dbo].[MA_Objects_Delta] d
            LEFT JOIN  [dbo].[MA_Objects] o ON 
                       d.[objectId] = o.[objectId]
            WHERE	   d.[rowversion] <= @watermark
END
GO
    
INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 6, 'Update to delta stored procedure', GETDATE())
