-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2015
-- Description:	Gets the next value from the specified sequence
-- =============================================
CREATE PROCEDURE [dbo].[spSequenceGetNextValue]
	@sequenceName nvarchar(50)
AS
	SET NOCOUNT ON;
	DECLARE @nextValue bigint;

	DECLARE @sql nvarchar(1000) = N'SET @nextValueOUT = NEXT VALUE FOR [dbo].' + QUOTENAME(@sequenceName);
	DECLARE @params nvarchar(100) = N'@nextValueOUT bigint OUTPUT';
	EXEC sp_executesql @sql, @params, @nextValueOUT = @nextValue OUTPUT;

	return @nextValue;
