-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Determines if an attribute exists in the MA_Attributes table
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaDoesMVAttributeExist]
	@attributeName nvarchar(50) 
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT 1 FROM [dbo].[MA_Attributes] WHERE [attributeName]=@attributeName)
		BEGIN
			SELECT 1;
			RETURN;
		END
	ELSE
		BEGIN
			SELECT 0;
			RETURN;
		END
END
