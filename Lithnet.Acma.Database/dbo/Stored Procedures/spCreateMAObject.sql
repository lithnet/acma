-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spCreateMAObject]
@id uniqueidentifier,
@objectClass nvarchar(50),
@shadowParent uniqueidentifier = null
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[MA_Objects] (objectId, objectClass, shadowParent) VALUES (@id, @objectClass, @shadowParent);

	SELECT TOP 1 * FROM [dbo].[MA_Objects]
	WHERE [objectId] = @id;
END
