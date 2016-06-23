DECLARE @majorReleaseNumber int;
DECLARE @minorReleaseNumber int;
DECLARE @pointReleaseNumber int;
DECLARE @scriptName nvarchar(20);

SET @majorReleaseNumber=1;
SET @minorReleaseNumber=5;
SET	@pointReleaseNumber=4;
SET @scriptName=N'Setup_1.5.4';

IF NOT EXISTS(
    SELECT 1 FROM [$(DatabaseName)].[dbo].[DB_Version] WHERE
        MajorReleaseNumber = @majorReleaseNumber AND
        MinorReleaseNumber = @minorReleaseNumber AND
        PointReleaseNumber = @pointReleaseNumber)
        BEGIN
            INSERT INTO [$(DatabaseName)].[dbo].[DB_Version]
                (MajorReleaseNumber, MinorReleaseNumber, PointReleaseNumber, ScriptName, DateApplied)
                VALUES
                (@majorReleaseNumber, @minorReleaseNumber, @pointReleaseNumber, @scriptName, GETDATE())
        END
