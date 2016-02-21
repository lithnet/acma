-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSequenceDelete]
	-- Add the parameters for the stored procedure here
	@sequenceName nvarchar(128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @sql nvarchar(500) = N'DROP SEQUENCE [dbo].' + quotename(@sequenceName);

	EXECUTE sp_executesql @sql;
END
