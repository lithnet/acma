/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT

USE AttributeConstructorMA
BEGIN TRANSACTION
GO
CREATE TABLE dbo.MA_Objects 
	(
	id uniqueidentifier NOT NULL ROWGUIDCOL,
	mvDeleted timestamp NOT NULL,
	objectClass nvarchar(255) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.MA_Objects ADD CONSTRAINT
	DF_MA_Objects_id DEFAULT (newid()) FOR id
GO
ALTER TABLE dbo.MA_Objects ADD CONSTRAINT
	PK_MA_Objects PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MA_Objects SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
