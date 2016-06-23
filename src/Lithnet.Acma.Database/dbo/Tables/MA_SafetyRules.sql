CREATE TABLE [dbo].[MA_SafetyRules] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [AttributeID] INT            NOT NULL,
    [Pattern]     NVARCHAR (MAX) NOT NULL,
    [NullAllowed] BIT            NOT NULL,
    CONSTRAINT [PK_MA_SafetyRules] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MA_SafetyRules_MA_SchemaAttributes] FOREIGN KEY ([AttributeID]) REFERENCES [dbo].[MA_SchemaAttributes] ([ID]) ON DELETE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_SafetyRules_Name]
    ON [dbo].[MA_SafetyRules]([Name] ASC);

