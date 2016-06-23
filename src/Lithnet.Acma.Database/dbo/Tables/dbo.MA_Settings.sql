CREATE TABLE [dbo].[MA_Settings] (
    [Name]  NVARCHAR (50)  NOT NULL,
    [Value] NVARCHAR (400) NULL,
    CONSTRAINT [PK_MA_Settings] PRIMARY KEY CLUSTERED ([Name] ASC)
);

