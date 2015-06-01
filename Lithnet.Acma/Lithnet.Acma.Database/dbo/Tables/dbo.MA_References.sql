CREATE TABLE [dbo].[MA_References] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [objectId]      UNIQUEIDENTIFIER NOT NULL,
    [attributeName] NVARCHAR (50)    NOT NULL,
    [value]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_MA_References] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MA_References_MA_Objects] FOREIGN KEY ([objectId]) REFERENCES [dbo].[MA_Objects] ([objectId]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_MA_References_MA_SchemaAttributes] FOREIGN KEY ([attributeName]) REFERENCES [dbo].[MA_SchemaAttributes] ([Name]) ON DELETE CASCADE ON UPDATE CASCADE
);




GO











GO



GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_References_objectID_attributeName]
    ON [dbo].[MA_References]([objectId] ASC, [attributeName] ASC, [value] ASC);


GO
CREATE TRIGGER [dbo].[trigger_modify_MA_References]
ON [dbo].[MA_References]
FOR INSERT, DELETE, UPDATE AS
SET NOCOUNT ON
INSERT INTO [dbo].[MA_Objects_Delta]
	SELECT DISTINCT inserted.[objectId], 'modify', null
	FROM inserted LEFT OUTER JOIN [dbo].[MA_Objects_Delta] ON
	inserted.[objectId] = [dbo].[MA_Objects_Delta].[objectId]
	WHERE ([dbo].[MA_Objects_Delta].[objectId] IS NULL)
GO
CREATE NONCLUSTERED INDEX [IX_MA_References_value]
    ON [dbo].[MA_References]([value] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_References_attributeName_value]
    ON [dbo].[MA_References]([attributeName] ASC, [value] ASC);

