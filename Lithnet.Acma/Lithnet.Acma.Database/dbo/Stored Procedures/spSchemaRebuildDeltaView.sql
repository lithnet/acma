-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Updates the definition of the delta view
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaRebuildDeltaView]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @sql nvarchar(4000) = 
		N'ALTER VIEW [dbo].[v_MA_Objects_Delta] AS
			SELECT     [dbo].[MA_Objects].*, 
					   [dbo].[MA_Objects_Delta].[operation] as [operation], 
					   [dbo].[MA_Objects_Delta].[rowversion] as [rowversiondelta]
			FROM       [dbo].[MA_Objects_Delta]
			LEFT JOIN  [dbo].[MA_Objects] ON 
					   [dbo].[MA_Objects_Delta].[objectId] = [dbo].[MA_Objects].[objectId]'

	EXEC sp_executesql @sql;
END
