
-- =============================================
-- Author:		Ryan Newington
-- Create date: 2/09/2014
-- Description:	Gets all objects of a specified object class
-- =============================================
CREATE PROCEDURE [dbo].[spGetObjectsOfClass]
	@objectClass nvarchar(50)
AS
BEGIN
	DECLARE @id INT
	DECLARE @name NVARCHAR(100)
	DECLARE @getid CURSOR
	DECLARE @columns NVARCHAR(MAX)
	SET @columns = 'id'

	SET @getid = CURSOR FOR
		SELECT 
			a.[ColumnName]
		FROM
			 [dbo].[MA_SchemaMapping] m
		JOIN
			 [dbo].[MA_SchemaObjectClasses] o
		ON 
			m.[ObjectClassID] = o.[ID]
		JOIN 
			[dbo].[MA_SchemaAttributes] a
		ON 
			a.[ID] = m.[AttributeID]
		WHERE 
			o.[Name] = @objectClass
			AND m.[InheritanceSourceAttributeID] is null
			AND a.[IsMultivalued] = 0
			AND a.[Type] != 2

	OPEN @getid
	FETCH NEXT FROM @getid INTO @name
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @columns = @columns + ', ' + QUOTENAME(@name)
		FETCH NEXT FROM @getid INTO @name
	END

	CLOSE @getid
	DEALLOCATE @getid

	DECLARE @params nvarchar(200) = N'@objectClass nvarchar(50)'
	DECLARE @sql nvarchar(max) = N'SELECT ' + @columns + N' FROM MA_Objects where objectClass=@objectClass'
	EXEC sp_executesql @sql, @params, @objectClass
END