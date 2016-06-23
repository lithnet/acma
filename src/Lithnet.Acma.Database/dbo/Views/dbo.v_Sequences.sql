CREATE VIEW [dbo].[v_Sequences] AS
	SELECT 
		seq.[object_id] as ID,
		name as Name,
		is_cycling as IsCycleEnabled,
		CAST(start_value as bigint) as StartValue,
		CAST(minimum_value as bigint) as MinValue,
		CAST(maximum_value as bigint) as MaxValue,
		CAST(current_value as bigint) as CurrentValue,
		CAST(increment as bigint) as Increment
	FROM
		[sys].sequences AS seq
