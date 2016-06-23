-- =============================================
-- Author:		Ryan Newington
-- Create date: 19/1/2014
-- Description:	Gets all MV attributes associated with an MAObject
-- =============================================
CREATE PROCEDURE [dbo].[spGetMAObjectMVAttributes]
	@objectId uniqueidentifier,
	@attributeName nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [dbo].[MA_Attributes].* FROM [dbo].[MA_Attributes] WHERE [objectId]=@objectId AND [attributeName]=@attributeName
END
