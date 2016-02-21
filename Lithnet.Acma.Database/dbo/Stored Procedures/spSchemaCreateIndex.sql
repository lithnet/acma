-- =============================================
-- Author:		Ryan Newington
-- Create date: 22-01-2014
-- Description:	Creates an index on the MA_Objects table
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaCreateIndex]
	@attributeName nvarchar(50) 
AS
BEGIN
	DECLARE @sql nvarchar(4000);
	SET NOCOUNT ON;

	DECLARE @isMultiValued bit;
	DECLARE @operation int;

	SELECT @isMultiValued = IsMultiValued 
	FROM dbo.MA_SchemaAttributes WHERE Name = @attributeName;
	
	SELECT @operation = Operation
	FROM dbo.MA_SchemaAttributes WHERE Name = @attributeName;
	
	IF (@isMultiValued = 1)
		THROW 50010, N'Cannot create an index on a multivalued attribute', 1;
	
	IF (@operation = 4)
		THROW 50011, N'Cannot create an index on a temporary attribte',1;
		
	BEGIN TRANSACTION
		EXEC spSchemaDeleteIndex @attributeName;
		SET @sql = N'CREATE NONCLUSTERED INDEX IX_MA_Objects_' + @attributeName + N' ON dbo.MA_Objects (' + @attributeName + N') 
				WITH (STATISTICS_NORECOMPUTE = OFF, 
						IGNORE_DUP_KEY = OFF, 
						ALLOW_ROW_LOCKS = ON, 
						ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]'
		EXEC sp_executesql @sql

		UPDATE dbo.MA_SchemaAttributes
		SET IsIndexed = 1
		WHERE Name = @attributeName

	COMMIT TRANSACTION
END
