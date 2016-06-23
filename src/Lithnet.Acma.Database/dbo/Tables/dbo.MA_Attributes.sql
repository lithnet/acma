CREATE TABLE [dbo].[MA_Attributes] (
    [id]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [objectId]               UNIQUEIDENTIFIER NOT NULL,
    [attributeName]          NVARCHAR (50)    NOT NULL,
    [attributeValueStringIX] NVARCHAR (400)   NULL,
    [attributeValueInt]      BIGINT           NULL,
    [attributeValueBinaryIX] VARBINARY (800)  NULL,
    [attributeValueString]   NVARCHAR (MAX)   NULL,
    [attributeValueBinary]   VARBINARY (MAX)  NULL,
    [attributeValueDateTime] DATETIME2 (3)    NULL,
    CONSTRAINT [PK_MA_Attributes] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_MA_Attributes_MA_Objects] FOREIGN KEY ([objectId]) REFERENCES [dbo].[MA_Objects] ([objectId]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_MA_Attributes_MA_SchemaAttributes] FOREIGN KEY ([attributeName]) REFERENCES [dbo].[MA_SchemaAttributes] ([Name]) ON DELETE CASCADE ON UPDATE CASCADE
);








GO









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



GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_attributeName_attributeValueBinary]
    ON [dbo].[MA_Attributes]([attributeName] ASC, [attributeValueBinaryIX] ASC)
    INCLUDE([objectId]);


GO
CREATE TRIGGER [dbo].[trigger_modify_MA_Attributes]
ON [dbo].[MA_Attributes]
FOR INSERT, UPDATE AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;

        DECLARE cur CURSOR LOCAL FOR
        SELECT [inserted].[objectId] FROM [inserted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'modify';
            FETCH NEXT FROM cur INTO @objectId
        END

        CLOSE cur
        DEALLOCATE cur
    END

GO
CREATE NONCLUSTERED INDEX [IX_MA_Attributes_attributeName_attributeValueDateTime]
    ON [dbo].[MA_Attributes]([attributeName] ASC, [attributeValueDateTime] ASC);


GO
CREATE TRIGGER [dbo].[trigger_delete_MA_Attributes]
ON [dbo].[MA_Attributes]
FOR DELETE AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;

        DECLARE cur CURSOR LOCAL FOR
        SELECT [deleted].[objectId] FROM [deleted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'modify';
            FETCH NEXT FROM cur INTO @objectId
        END

        CLOSE cur
        DEALLOCATE cur
    END