-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2015
-- Description:	Modifies the parameters of a database sequence
-- =============================================
CREATE PROCEDURE [dbo].[spSequenceModify]
    @sequenceName nvarchar(128),
    @newSequenceName nvarchar(128),
    @restartWith bigint,
    @incrementBy bigint,
    @minValue bigint,
    @maxValue bigint,
    @cycle bit
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sql nvarchar(500);

    set @sql =N'ALTER SEQUENCE [dbo].' + quotename(@sequenceName) + N' 
    RESTART WITH ' + CAST(@restartWith as nvarchar(50))  + N'
    INCREMENT BY ' + CAST(@incrementBy as nvarchar(50)) 
    
    IF (@minValue is null)
        SET @sql = @SQL + N' NO MINVALUE'
    ELSE
        SET @sql = @sql + N' MINVALUE ' + CAST(@minValue as nvarchar(50)) 

    IF (@maxValue is null)
        SET @sql = @sql + N' NO MAXVALUE'
    ELSE
        SET @sql = @sql + N' MAXVALUE ' + CAST(@maxValue as nvarchar(50)) 

    IF (@cycle = 1)
        SET @sql = @sql + N' CYCLE'
    ELSE
        SET @sql = @sql + N' NO CYCLE'

    EXECUTE sp_executesql @sql;

    IF (@newSequenceName is not null)
        IF (@newSequenceName != @sequenceName)
            BEGIN
                DECLARE @existingName nvarchar(150) = '[dbo].' + quotename(@sequenceName);
                EXECUTE sp_rename @existingName, @newSequenceName;
            END

END
