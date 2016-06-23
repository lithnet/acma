CREATE VIEW [dbo].[v_MA_Objects_Delta] AS
			SELECT     [dbo].[MA_Objects].*, 
					   [dbo].[MA_Objects_Delta].[operation] as [operation], 
					   [dbo].[MA_Objects_Delta].[rowversion] as [rowversiondelta]
			FROM       [dbo].[MA_Objects_Delta]
			LEFT JOIN  [dbo].[MA_Objects] ON 
					   [dbo].[MA_Objects_Delta].[objectId] = [dbo].[MA_Objects].[objectId]