-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2015
-- Description:	Gets a sequence definition from the database
-- =============================================
CREATE PROCEDURE [dbo].[spSequenceGet]
AS
BEGIN
	SET NOCOUNT ON;

   SELECT
		seq.name AS [Sequence Name],
		CAST(seq.precision AS int) AS [NumericPrecision],
		ISNULL(seq.start_value,N'''') AS [StartValue],
		ISNULL(seq.increment,N'''') AS [IncrementValue],
		ISNULL(seq.minimum_value,N'''') AS [MinValue],
		ISNULL(seq.maximum_value,N'''') AS [MaxValue],
		CAST(seq.is_cycling AS bit) AS [IsCycleEnabled],
		ISNULL(seq.cache_size,0) AS [CacheSize],
		ISNULL(seq.current_value,N'''') AS [CurrentValue]
	FROM
		[sys].sequences AS seq

END
