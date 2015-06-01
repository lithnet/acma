-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets database objects up to the specified watermark value
-- =============================================
CREATE PROCEDURE [dbo].[spGetMAObjects]
@watermark rowversion = NULL,
@deleted bit = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF (@deleted = 0)
		IF (@watermark IS NULL)
			SELECT * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[deleted] = 0
		ELSE
			SELECT * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[deleted] = 0 AND [dbo].[MA_Objects].[rowversion] <= @watermark
	ELSE
		IF (@watermark is NULL)
			SELECT * FROM [dbo].[MA_Objects] 
		ELSE
			SELECT * FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[rowversion] <= @watermark
		
END
