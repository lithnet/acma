-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetMAObjectsDelta]
@watermark rowversion = NULL,
@deleted bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF (@deleted = 0)
		IF (@watermark IS NULL)
			SELECT * FROM [dbo].[v_MA_Objects_Delta] WHERE [deleted] = 0
		ELSE
			SELECT * FROM [dbo].[v_MA_Objects_Delta] WHERE [deleted] = 0 AND [rowversiondelta] <= @watermark
	ELSE
		IF (@watermark IS NULL)
			SELECT * FROM [dbo].[v_MA_Objects_Delta] 
		ELSE
			SELECT * FROM [dbo].[v_MA_Objects_Delta] WHERE [rowversiondelta] <= @watermark
		
END
