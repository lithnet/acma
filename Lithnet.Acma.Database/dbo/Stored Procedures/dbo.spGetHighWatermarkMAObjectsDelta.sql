-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Gets the highest value for the rowversion
--				column of the MA_Objects_Delta table				
-- =============================================
CREATE PROCEDURE [dbo].[spGetHighWatermarkMAObjectsDelta]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT MAX([dbo].[MA_Objects_Delta].[rowversion]) FROM [dbo].[MA_Objects_Delta]
END
