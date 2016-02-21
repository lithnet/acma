CREATE TABLE [dbo].[MA_SchemaObjectClasses] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (50)  NOT NULL,
    [ShadowFromObjectClassID] INT            NULL,
    [AllowResurrection]       BIT            CONSTRAINT [DF_MA_SchemaObjectClasses_AllowResurrection] DEFAULT ((0)) NOT NULL,
    [Description]             NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_MA_SchemaObjects_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MA_SchemaObjectClasses_MA_SchemaObjectClasses] FOREIGN KEY ([ShadowFromObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID]),
    CONSTRAINT [FK_MA_SchemaObjects_MA_SchemaObjects] FOREIGN KEY ([ShadowFromObjectClassID]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([ID])
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_SchemaObjects_Name]
    ON [dbo].[MA_SchemaObjectClasses]([Name] ASC);

