-- =============================================
-- Author:		Ryan Newington
-- Create date: 30-05-2014
-- Description:	Renames an attribute in the schema
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaRenameAttribute]
    @attributeName nvarchar(50),
    @newAttributeName nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        SET QUOTED_IDENTIFIER ON
        SET ARITHABORT ON
        SET NUMERIC_ROUNDABORT OFF
        SET CONCAT_NULL_YIELDS_NULL ON
        SET ANSI_NULLS ON
        SET ANSI_PADDING ON
        SET ANSI_WARNINGS ON
    COMMIT

    DECLARE @isMultiValued bit;
    DECLARE @isIndexed bit;
    DECLARE @tableName nvarchar(50);
	DECLARE @type int;

    SELECT 
		@isIndexed = [dbo].[MA_SchemaAttributes].[IsIndexed],
		@tableName = [dbo].[MA_SchemaAttributes].[TableName],
		@type = [dbo].[MA_SchemaAttributes].[Type]
    FROM [dbo].[MA_SchemaAttributes] WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName;

    BEGIN TRANSACTION
        IF (@tableName = 'MA_Objects')
            BEGIN
                IF (@isIndexed = 1)
                    EXEC [dbo].spSchemaDeleteIndex @attributeName;

                DECLARE @columnName nvarchar(150);
                SET @columnName = 'MA_Objects.' + @attributeName;
                EXEC [dbo].[sp_rename] @columnName, @newAttributeName, 'COLUMN';
            
				IF (@type = 4)
				BEGIN
					declare @constraintName nvarchar(100) = N'[DF_MA_Objects_' + @attributeName + ']'
					IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@constraintName) AND type = 'D')
						BEGIN
							DECLARE @oldDefaultName nvarchar(100) = N'DF_MA_Objects_' + @attributeName;
							DECLARE @newDefaultName nvarchar(100) = N'DF_MA_Objects_' + @newAttributeName;
							exec sp_rename @oldDefaultName, @newDefaultName, 'object'
						END
				END

                IF (@isIndexed = 1)
                    EXEC [dbo].spSchemaCreateIndex @newAttributeName;

				UPDATE [dbo].[MA_SchemaAttributes]
				SET 
					[dbo].[MA_SchemaAttributes].[ColumnName] = @newAttributeName,
					[dbo].[MA_SchemaAttributes].[Name] = @newAttributeName
				WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName
            END
		ELSE
			BEGIN
				UPDATE [dbo].[MA_SchemaAttributes]
				SET [dbo].[MA_SchemaAttributes].[Name] = @newAttributeName
				WHERE [dbo].[MA_SchemaAttributes].[Name] = @attributeName
			END

    COMMIT TRANSACTION
END
