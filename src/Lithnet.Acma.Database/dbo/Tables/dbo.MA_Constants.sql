CREATE TABLE [dbo].[MA_Constants] (
    [ID]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (50)  NOT NULL,
    [Value] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_MA_Constants_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_Constants]
    ON [dbo].[MA_Constants]([Name] ASC);

