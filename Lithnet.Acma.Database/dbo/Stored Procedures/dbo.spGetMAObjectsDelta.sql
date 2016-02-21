-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the database objects from the delta table up to the specified watermark
-- =============================================
CREATE PROCEDURE [dbo].[spGetMAObjectsDelta]
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
