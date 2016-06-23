/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
			   SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DECLARE @majorReleaseNumber int;
DECLARE @minorReleaseNumber int;
DECLARE @pointReleaseNumber int;
DECLARE @scriptName nvarchar(20);

SET @majorReleaseNumber=1;
SET @minorReleaseNumber=7;
SET	@pointReleaseNumber=2;
SET @scriptName=N'Setup';

IF NOT EXISTS(
	SELECT 1 FROM [dbo].[DB_Version] WHERE
		MajorReleaseNumber = @majorReleaseNumber AND
		MinorReleaseNumber = @minorReleaseNumber AND
		PointReleaseNumber = @pointReleaseNumber)
		BEGIN
			INSERT INTO [dbo].[DB_Version]
				(MajorReleaseNumber, MinorReleaseNumber, PointReleaseNumber, ScriptName, DateApplied)
				VALUES
				(@majorReleaseNumber, @minorReleaseNumber, @pointReleaseNumber, @scriptName, GETDATE())
		END

INSERT INTO [dbo].[MA_SchemaAttributes] ([Name], [Type], [IsMultivalued], [Operation], [IsIndexable], [IsIndexed], [IsBuiltIn], [TableName], [ColumnName])
VALUES 
('deleted',      1, 0, 3, 0, 1, 1, 'MA_Objects', 'deleted'),
('objectClass',  0, 0, 3, 1, 1, 1, 'MA_Objects', 'objectClass'),
('objectId',     0, 0, 2, 1, 1, 1, 'MA_Objects', 'objectId'),
('shadowParent', 2, 0, 2, 1, 1, 1, 'MA_Objects', 'shadowParent'),
('shadowLink',   0, 0, 2, 0, 0, 1, 'MA_Objects', 'shadowLink')

	
