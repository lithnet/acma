-- =============================================
-- Author:		Ryan Newington
-- Create date: 03/03/2015
-- Description:	Creates a new object of the specified class
-- =============================================
CREATE PROCEDURE [dbo].[spCreateMAObject]
@id uniqueidentifier,
@objectClass nvarchar(50),
@shadowParent uniqueidentifier = null
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[MA_Objects] ([dbo].[MA_Objects].[objectId], [dbo].[MA_Objects].[objectClass], [dbo].[MA_Objects].[shadowParent]) VALUES (@id, @objectClass, @shadowParent);

	SELECT TOP 1 * FROM [dbo].[MA_Objects]
	WHERE [dbo].[MA_Objects].[objectId] = @id;
END
