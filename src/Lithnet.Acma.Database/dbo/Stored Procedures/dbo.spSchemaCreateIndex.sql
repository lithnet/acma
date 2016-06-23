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

    DECLARE @operation int;
    DECLARE @tableName nvarchar(50);

    SELECT  @operation = [dbo].[MA_SchemaAttributes].[Operation],
            @tableName = [dbo].[MA_SchemaAttributes].[TableName]
    FROM [dbo].[MA_SchemaAttributes] WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName;
    
    IF (@tableName != 'MA_Objects')
        THROW 50010, N'Cannot modify the index of an object that is not in the MA_Objects table', 1;

    IF (@operation = 4)
        THROW 50011, N'Cannot create an index on a temporary attribute',1;
        
    BEGIN TRANSACTION
        EXEC [dbo].[spSchemaDeleteIndex] @attributeName;
        SET @sql = N'CREATE NONCLUSTERED INDEX IX_MA_Objects_' + @attributeName + N' ON [dbo].[MA_Objects] (' + @attributeName + N') 
                WITH (STATISTICS_NORECOMPUTE = OFF, 
                        IGNORE_DUP_KEY = OFF, 
                        ALLOW_ROW_LOCKS = ON, 
                        ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]'
        EXEC sp_executesql @sql

        UPDATE dbo.MA_SchemaAttributes
        SET [dbo].[MA_SchemaAttributes].[IsIndexed] = 1
        WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName

    COMMIT TRANSACTION
END
