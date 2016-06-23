-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Clears the delta table from the specified watermark
-- =============================================
CREATE PROCEDURE [dbo].[spClearMAObjectsDelta]
@watermark rowversion
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[MA_Objects_Delta] WHERE [dbo].[MA_Objects_Delta].[rowversion] <= @watermark
END
