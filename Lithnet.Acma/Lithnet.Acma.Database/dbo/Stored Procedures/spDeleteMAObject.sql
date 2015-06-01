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

	DELETE FROM [dbo].[MA_Objects] WHERE [objectId]=@id;
	DELETE FROM [dbo].[MA_Objects_Delta] WHERE [objectId]=@id;
END
