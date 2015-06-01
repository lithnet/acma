-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetMAObjects]
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
			SELECT * FROM [dbo].[MA_Objects] WHERE [deleted] = 0
		ELSE
			SELECT * FROM [dbo].[MA_Objects] WHERE [deleted] = 0 AND [rowversion] <= @watermark
	ELSE
		IF (@watermark is NULL)
			SELECT * FROM [dbo].[MA_Objects] 
		ELSE
			SELECT * FROM [dbo].[MA_Objects] WHERE [rowversion] <= @watermark
		
END
