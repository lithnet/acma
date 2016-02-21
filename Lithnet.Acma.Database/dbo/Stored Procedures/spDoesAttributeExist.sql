-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDoesAttributeExist]
@objectId uniqueidentifier,
@attributeName nvarchar(50),
@isMultiValued bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @SQLString nvarchar(1000);
	DECLARE @ParmDefinition nvarchar(500);
	DECLARE @resultOutput int;

    -- Insert statements for procedure here
	IF (@isMultiValued = 0)
		BEGIN
		SET @SQLString = N'SELECT @result=1 FROM [dbo].[MA_Objects] WHERE (' + QUOTENAME(@attributeName) +' IS NOT NULL AND [id]=@objectId)'
		SET @ParmDefinition = N'@objectId uniqueidentifier, @result int OUTPUT';
		EXEC sp_executesql @SQLString, @ParmDefinition, @objectId, @resultOutput OUTPUT;
			IF (@resultOutput = 1)
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
	ELSE
		BEGIN
			IF EXISTS(SELECT 1 FROM [dbo].[MA_Attributes] WHERE ([attributeName]=@attributeName AND [objectId]=@objectId))
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
	
END
