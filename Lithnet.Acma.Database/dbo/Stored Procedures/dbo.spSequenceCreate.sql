-- =============================================
-- Author:		Ryan Newington
-- Create date: 04/06/2014
-- Description:	Creates a new sequence in the database
-- =============================================
CREATE PROCEDURE [dbo].[spSequenceCreate]
    @sequenceName nvarchar(128),
    @startsWith bigint,
    @incrementBy bigint,
    @minValue bigint,
    @maxValue bigint,
    @cycle bit

AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql nvarchar(max);

    set @sql =N'CREATE SEQUENCE [dbo].' + quotename(@sequenceName) + N' 
    AS bigint
    START WITH ' + CAST(@startsWith as nvarchar(50))  + N'
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

END
