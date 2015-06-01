-- =============================================
-- Author:		Ryan Newington
-- Create date: 30-05-2014
-- Description:	Renames an object class in the schema
-- =============================================
CREATE PROCEDURE [dbo].[spSchemaRenameObjectClass]
    @className nvarchar(50),
    @newClassName nvarchar(50)
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

        BEGIN TRANSACTION
                
            UPDATE [dbo].[MA_SchemaObjectClasses]
            SET [dbo].[MA_SchemaObjectClasses].[Name] = @newClassName
            WHERE [dbo].[MA_SchemaObjectClasses].[Name] = @className

        COMMIT TRANSACTION
END
