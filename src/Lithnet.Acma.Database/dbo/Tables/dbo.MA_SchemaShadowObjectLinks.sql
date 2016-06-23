CREATE TABLE [dbo].[MA_SchemaShadowObjectLinks] (
    [ID]                      INT           IDENTITY (1, 1) NOT NULL,
    [ParentObjectClassID]     INT           NOT NULL,
    [ShadowObjectClassID]     INT           NOT NULL,
    [ProvisioningAttributeID] INT           NOT NULL,
    [ReferenceAttributeID]    INT           NOT NULL,
    [Name]                    NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MA_SchemaShadowObjectStorage] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MA_SchemaShadowObjectLinks_MA_SchemaAttributes] FOREIGN KEY ([ProvisioningAttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaShadowObjectLinks_MA_SchemaAttributes1] FOREIGN KEY ([ReferenceAttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaShadowObjectLinks_MA_SchemaObjectClasses] FOREIGN KEY ([ShadowObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID]),
    CONSTRAINT [FK_MA_SchemaShadowObjectLinks_MA_SchemaObjects] FOREIGN KEY ([ParentObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID]),
    CONSTRAINT [IX_MA_SchemaShadowObjectLinks] UNIQUE NONCLUSTERED ([ParentObjectClassID] ASC, [ProvisioningAttributeID] ASC),
    CONSTRAINT [IX_MA_SchemaShadowObjectLinks_1] UNIQUE NONCLUSTERED ([ParentObjectClassID] ASC, [ReferenceAttributeID] ASC),
    CONSTRAINT [IX_MA_SchemaShadowObjectLinks_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_SchemaShadowObjectLinks_2]
    ON [dbo].[MA_SchemaShadowObjectLinks]([ParentObjectClassID] ASC, [ProvisioningAttributeID] ASC, [ReferenceAttributeID] ASC, [ShadowObjectClassID] ASC);

