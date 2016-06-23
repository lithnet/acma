-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [spSequenceGet]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
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
