-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the high watermark from the MA_Objects table
-- =============================================
CREATE PROCEDURE [dbo].[spGetHighWatermarkMAObjects]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MAX([dbo].[MA_Objects].[rowversion]) FROM [dbo].[MA_Objects]
END
