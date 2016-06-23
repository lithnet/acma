CREATE TABLE [dbo].[MA_SchemaReferenceLinks] (
    [ID]                  INT IDENTITY (1, 1) NOT NULL,
    [SourceObjectClassID] INT NOT NULL,
    [SourceAttributeID]   INT NOT NULL,
    [TargetAttributeID]   INT NOT NULL,
    [TargetObjectClassID] INT NOT NULL,
    CONSTRAINT [PK_MA_SchemaReferenceLinks] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MA_SchemaReferenceLinks_MA_SchemaAttributes] FOREIGN KEY ([TargetAttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaReferenceLinks_MA_SchemaObjects] FOREIGN KEY ([SourceAttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaReferenceLinks_MA_SchemaObjects1] FOREIGN KEY ([TargetObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID]),
    CONSTRAINT [FK_MA_SchemaReferenceLinks_MA_SchemaObjects2] FOREIGN KEY ([SourceObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_SchemaReferenceLinks]
    ON [dbo].[MA_SchemaReferenceLinks]([SourceAttributeID] ASC, [SourceObjectClassID] ASC, [TargetAttributeID] ASC, [TargetObjectClassID] ASC);

