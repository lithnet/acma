CREATE TABLE [dbo].[MA_SchemaAttributes] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50) NOT NULL,
    [Type]          INT           CONSTRAINT [DF_MA_SchemaAttributes_Type] DEFAULT ((0)) NOT NULL,
    [IsMultivalued] BIT           CONSTRAINT [DF_MA_SchemaAttributes_Multivalued] DEFAULT ((0)) NOT NULL,
    [Operation]     INT           CONSTRAINT [DF_MA_SchemaAttributes_Operation] DEFAULT ((0)) NOT NULL,
    [IsIndexable]   BIT           CONSTRAINT [DF_MA_SchemaAttributes_Indexable] DEFAULT ((0)) NOT NULL,
    [IsIndexed]     BIT           CONSTRAINT [DF_MA_SchemaAttributes_Indexed] DEFAULT ((0)) NOT NULL,
    [IsBuiltIn]     BIT           CONSTRAINT [DF_MA_SchemaAttributes_IsBuiltIn] DEFAULT ((0)) NOT NULL,
    [TableName]     NVARCHAR (50) NULL,
    [ColumnName]    NVARCHAR (50) NULL,
    CONSTRAINT [PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Name]
    ON [dbo].[MA_SchemaAttributes]([Name] ASC);

