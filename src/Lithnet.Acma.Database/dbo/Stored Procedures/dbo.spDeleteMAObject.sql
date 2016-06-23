-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteMAObject]
@id uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[MA_Objects] WHERE [dbo].[MA_Objects].[objectId]=@id;
END
