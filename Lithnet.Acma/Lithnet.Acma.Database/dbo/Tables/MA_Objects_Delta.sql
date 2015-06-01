CREATE TABLE [dbo].[MA_Objects_Delta] (
    [id]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [objectId]   UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [operation]  NVARCHAR (10)    NOT NULL,
    [rowversion] ROWVERSION       NOT NULL,
    CONSTRAINT [PK_MA_Objects_Delta_1] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_Delta_objectId]
    ON [dbo].[MA_Objects_Delta]([objectId] ASC);

