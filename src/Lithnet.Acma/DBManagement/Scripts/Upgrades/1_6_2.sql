ALTER TABLE dbo.MA_Attributes
    DROP CONSTRAINT FK_MA_Attributes_MA_Objects

ALTER TABLE dbo.MA_References
    DROP CONSTRAINT FK_MA_References_MA_Objects

IF EXISTS (SELECT * FROM sys.indexes WHERE name='IX_MA_Objects_objectId' AND object_id = OBJECT_ID('[dbo].[MA_Objects]'))
    DROP INDEX IX_MA_Objects_objectId ON dbo.MA_Objects

IF EXISTS (SELECT * FROM sys.indexes WHERE name='IX_MA_Objects_ID' AND object_id = OBJECT_ID('[dbo].[MA_Objects]'))
    DROP INDEX IX_MA_Objects_ID ON dbo.MA_Objects

ALTER TABLE dbo.ma_objects DROP CONSTRAINT PK_MA_Objects

ALTER TABLE dbo.MA_Objects ADD CONSTRAINT
    PK_MA_Objects PRIMARY KEY NONCLUSTERED
    (
    objectId
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE CLUSTERED INDEX IX_MA_Objects_ID
    ON dbo.MA_Objects(id); 

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_MA_Objects_objectClass' AND object_id = OBJECT_ID('[dbo].[MA_Objects]'))
    CREATE NONCLUSTERED INDEX [IX_MA_Objects_objectClass]
        ON [dbo].[MA_Objects]([objectClass] ASC);

ALTER TABLE dbo.MA_Objects SET (LOCK_ESCALATION = TABLE)

ALTER TABLE dbo.MA_References ADD CONSTRAINT
    FK_MA_References_MA_Objects FOREIGN KEY
    (
    objectId
    ) REFERENCES dbo.MA_Objects
    (
    objectId
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
ALTER TABLE dbo.MA_References SET (LOCK_ESCALATION = TABLE)

ALTER TABLE dbo.MA_Attributes ADD CONSTRAINT
    FK_MA_Attributes_MA_Objects FOREIGN KEY
    (
    objectId
    ) REFERENCES dbo.MA_Objects
    (
    objectId
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
ALTER TABLE dbo.MA_Attributes SET (LOCK_ESCALATION = TABLE)

INSERT INTO [dbo].[DB_Version] ([MajorReleaseNumber], [MinorReleaseNumber], [PointReleaseNumber], [ScriptName], [DateApplied])
VALUES (1, 6, 2, 'Primary key update', GETDATE())
GO

PRINT N'Update complete.';
