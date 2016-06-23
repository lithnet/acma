CREATE TABLE [dbo].[MA_SchemaMapping] (
    [ID]                           INT IDENTITY (1, 1) NOT NULL,
    [ObjectClassID]                INT NOT NULL,
    [AttributeID]                  INT NOT NULL,
    [InheritanceSourceAttributeID] INT NULL,
    [InheritedAttributeID]         INT NULL,
    [IsBuiltIn]                    BIT CONSTRAINT [DF_MA_SchemaMapping_IsBuiltIn] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MA_SchemaMapping] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MA_SchemaMapping_MA_SchemaAttributes] FOREIGN KEY ([AttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaMapping_MA_SchemaAttributes1] FOREIGN KEY ([InheritanceSourceAttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaMapping_MA_SchemaAttributes2] FOREIGN KEY ([InheritedAttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]),
    CONSTRAINT [FK_MA_SchemaMapping_MA_SchemaObjects] FOREIGN KEY ([ObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UniqueSchemaObjectAndAttributePair]
    ON [dbo].[MA_SchemaMapping]([ObjectClassID] ASC, [AttributeID] ASC);

