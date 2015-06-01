CREATE TABLE [dbo].[MA_Attributes] (
    [id]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [objectId]                UNIQUEIDENTIFIER NOT NULL,
    [attributeName]           NVARCHAR (50)    NOT NULL,
    [attributeValueStringIX]  NVARCHAR (400)   NULL,
    [attributeValueInt]       BIGINT           NULL,
    [attributeValueBinaryIX]  VARBINARY (800)  NULL,
    [attributeValueReference] UNIQUEIDENTIFIER NULL,
    [attributeValueString]    NVARCHAR (MAX)   NULL,
    [attributeValueBinary]    VARBINARY (MAX)  NULL,
    CONSTRAINT [PK_MA_Attributes] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_MA_Attributes_MA_SchemaAttributes] FOREIGN KEY ([attributeName]) REFERENCES [dbo].[MA_SchemaAttributes] ([Name]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_objectId_AttributeName]
    ON [dbo].[MA_Attributes]([objectId] ASC, [attributeName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_attributeName_attributeValueString]
    ON [dbo].[MA_Attributes]([attributeName] ASC, [attributeValueStringIX] ASC)
    INCLUDE([objectId]);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_attributeName_attributeValueInt]
    ON [dbo].[MA_Attributes]([attributeName] ASC, [attributeValueInt] ASC)
    INCLUDE([objectId]);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_attributeName_attributeValueReference]
    ON [dbo].[MA_Attributes]([attributeName] ASC, [attributeValueReference] ASC)
    INCLUDE([objectId]);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_attributeName_attributeValueBinary]
    ON [dbo].[MA_Attributes]([attributeName] ASC, [attributeValueBinaryIX] ASC)
    INCLUDE([objectId]);


GO
CREATE TRIGGER dbo.trigger_modify_MA_Attributes
ON dbo.MA_Attributes
FOR INSERT, DELETE, UPDATE AS
SET NOCOUNT ON
INSERT INTO dbo.MA_Objects_Delta 
	SELECT DISTINCT inserted.objectId, 'modify', null
	FROM inserted LEFT OUTER JOIN dbo.MA_Objects_Delta ON
	inserted.ObjectId = dbo.MA_Objects_Delta.objectId
	WHERE (dbo.MA_Objects_Delta.objectId IS NULL)
