CREATE TABLE [dbo].[MA_Objects] (
    [id]                                INT              IDENTITY (1, 1) NOT NULL,
    [objectId]                          UNIQUEIDENTIFIER CONSTRAINT [DF_MA_Objects_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [objectClass]                       NVARCHAR (50)    NOT NULL,
    [deleted]                           BIGINT           CONSTRAINT [DF_MA_Objects_mvDeleted] DEFAULT ((0)) NOT NULL,
    [inheritedUpdate]                   BIT              CONSTRAINT [DF_MA_Objects_inheritedUpdate] DEFAULT ((0)) NOT NULL,
    [shadowLink]                        NVARCHAR (50)    NULL,
    [shadowParent]                      UNIQUEIDENTIFIER NULL,
    [rowversion]                        ROWVERSION       NOT NULL,
    CONSTRAINT [PK_MA_Objects] PRIMARY KEY NONCLUSTERED ([objectId] ASC),
    CONSTRAINT [FK_MA_Objects_MA_SchemaObjects] FOREIGN KEY ([objectClass]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([Name]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_ShadowObjectLinkName] FOREIGN KEY ([shadowLink]) REFERENCES [dbo].[MA_SchemaShadowObjectLinks] ([Name]) ON UPDATE CASCADE
);








GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_mvDeleted]
    ON [dbo].[MA_Objects]([deleted] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_MA_Objects_shadowParent]
    ON [dbo].[MA_Objects]([shadowParent] ASC);


GO



GO

GO
CREATE TRIGGER [dbo].[trigger_add_MA_Objects]
ON dbo.MA_Objects
FOR INSERT AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;

        DECLARE cur CURSOR LOCAL FOR
        SELECT [inserted].[objectId] FROM [inserted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'add';
            FETCH NEXT FROM cur INTO @objectId
        END

        CLOSE cur
        DEALLOCATE cur
    END

GO
CREATE TRIGGER [dbo].[trigger_modify_MA_Objects]
ON dbo.MA_Objects
FOR UPDATE AS
BEGIN
    SET NOCOUNT ON
        
    IF ((SELECT TRIGGER_NESTLEVEL()) > 1)
        RETURN;

    DECLARE @objectId uniqueidentifier;
    DECLARE @oldDeleted bigint;
    DECLARE @newDeleted bigint;

    DECLARE cur CURSOR LOCAL FOR
        SELECT [inserted].[objectId], [deleted].[deleted], [inserted].[deleted] FROM [inserted]
        INNER JOIN [deleted] ON [inserted].[objectId] = [deleted].[objectId];

    OPEN cur
    FETCH NEXT FROM cur into @objectId, @oldDeleted, @newDeleted

    WHILE @@FETCH_STATUS = 0 
    BEGIN
        DECLARE @changeType nvarchar(10) =
            CASE
                WHEN (@oldDeleted = 0 AND @newDeleted > 0) THEN N'delete'
                WHEN (@oldDeleted > 0 AND @newDeleted = 0) THEN N'add'
                ELSE N'modify'
            END;

        EXEC [dbo].[spCreateDeltaEntry] @objectId, @changeType;
        FETCH NEXT FROM cur INTO  @objectId, @oldDeleted, @newDeleted
    END

    CLOSE cur
    DEALLOCATE cur
END

GO
CREATE TRIGGER [dbo].[trigger_delete_MA_Objects]
ON dbo.MA_Objects
FOR DELETE AS
    BEGIN
        SET NOCOUNT ON
        DECLARE @objectId uniqueidentifier;

        DECLARE cur CURSOR LOCAL FOR
        SELECT [deleted].[objectId] FROM [deleted]

        OPEN cur
        FETCH NEXT FROM cur into @objectId

        WHILE @@FETCH_STATUS = 0 
        BEGIN
            EXEC [dbo].[spCreateDeltaEntry] @objectId, N'delete';
            FETCH NEXT FROM cur INTO @objectId
        END

        CLOSE cur
        DEALLOCATE cur
    END

GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[trigger_clearInheritedUpdate] ON dbo.MA_Objects
FOR INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[MA_Objects]
        SET 
            [dbo].[MA_Objects].[inheritedUpdate] = 0
        FROM 
            [inserted]
        WHERE 
            [inserted].[objectId] = [dbo].[MA_Objects].[objectId] AND
            [inserted].[inheritedUpdate] = 1

END

GO
