CREATE TABLE [dbo].[MA_Objects] (
    [id]                                 INT              IDENTITY (1, 1) NOT NULL,
    [objectId]                           UNIQUEIDENTIFIER CONSTRAINT [DF_MA_Objects_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [objectClass]                        NVARCHAR (50)    NOT NULL,
    [deleted]                            BIGINT           CONSTRAINT [DF_MA_Objects_mvDeleted] DEFAULT ((0)) NOT NULL,
    [inheritedUpdate]                    BIT              CONSTRAINT [DF_MA_Objects_inheritedUpdate] DEFAULT ((0)) NOT NULL,
    [shadowLink]                         INT              NULL,
    [shadowParent]                       UNIQUEIDENTIFIER NULL,
    [rowversion]                         ROWVERSION       NOT NULL,
    [accountName]                        NVARCHAR (400)   NULL,
    [activeExpiryDate]                   BIGINT           NULL,
    [adDn]                               NVARCHAR (400)   NULL,
    [adDomain]                           NVARCHAR (400)   NULL,
    [adGroupScopeName]                   NVARCHAR (400)   NULL,
    [adGroupType]                        BIGINT           NULL,
    [adGroupTypeName]                    NVARCHAR (400)   NULL,
    [adMgrAccount]                       UNIQUEIDENTIFIER NULL,
    [adUserPrincipalName]                NVARCHAR (400)   NULL,
    [businessUnitShort]                  NVARCHAR (400)   NULL,
    [callistaEncumbered]                 NVARCHAR (400)   NULL,
    [callistaEnrolmentStatus]            NVARCHAR (400)   NULL,
    [callistaExpiryDate]                 BIGINT           NULL,
    [callistaPersonId]                   NVARCHAR (400)   NULL,
    [callistaPersonIdOverrideRef]        UNIQUEIDENTIFIER NULL,
    [callistaProvisionStatus]            NVARCHAR (400)   NULL,
    [callistaRegistrationAuthcate]       NVARCHAR (400)   NULL,
    [callistaRegistrationEmail]          NVARCHAR (400)   NULL,
    [connectedToAD]                      BIT              NULL,
    [connectedToCallista]                BIT              NULL,
    [connectedToJmss]                    BIT              NULL,
    [connectedToSap]                     BIT              NULL,
    [connectedToZAHR]                    BIT              NULL,
    [displayName]                        NVARCHAR (400)   NULL,
    [displayNameUnique]                  NVARCHAR (400)   NULL,
    [dob]                                BIGINT           NULL,
    [enableUnixUid]                      BIT              NULL,
    [firstName]                          NVARCHAR (400)   NULL,
    [gender]                             NVARCHAR (400)   NULL,
    [givenNames]                         NVARCHAR (400)   NULL,
    [hasAdMgrAccount]                    BIT              NULL,
    [initials]                           NVARCHAR (400)   NULL,
    [jmssExpiryDate]                     BIGINT           NULL,
    [mail]                               NVARCHAR (400)   NULL,
    [mailAddressFormat]                  NVARCHAR (400)   NULL,
    [mailHost]                           NVARCHAR (400)   NULL,
    [mailRoutingAddress]                 NVARCHAR (400)   NULL,
    [middleName]                         NVARCHAR (400)   NULL,
    [monashPersonId]                     NVARCHAR (400)   NULL,
    [otpEmailAddress]                    NVARCHAR (400)   NULL,
    [otpMobileNumber]                    NVARCHAR (400)   NULL,
    [parentAccount]                      UNIQUEIDENTIFIER NULL,
    [parentAccountName]                  NVARCHAR (400)   NULL,
    [parentExpiryDate]                   BIGINT           NULL,
    [parentPerson]                       UNIQUEIDENTIFIER NULL,
    [personalTitle]                      NVARCHAR (400)   NULL,
    [provisionToAD]                      BIT              NULL,
    [provisionToGoogleApps]              BIT              NULL,
    [provisionToLds]                     BIT              NULL,
    [provisionToMds]                     BIT              NULL,
    [sapCampus]                          NVARCHAR (400)   NULL,
    [sapCampusArea]                      NVARCHAR (400)   NULL,
    [sapCentralId]                       NVARCHAR (400)   NULL,
    [sapCompanyNumber]                   NVARCHAR (400)   NULL,
    [sapCostCentre]                      NVARCHAR (400)   NULL,
    [sapEmployeeType]                    NVARCHAR (400)   NULL,
    [sapEmploymentStatus]                NVARCHAR (400)   NULL,
    [sapEndDate]                         BIGINT           NULL,
    [sapExpiryDate]                      BIGINT           NULL,
    [sapFundCode]                        NVARCHAR (400)   NULL,
    [sapOrganizationalUnit]              UNIQUEIDENTIFIER NULL,
    [sapPersonalEmail]                   NVARCHAR (400)   NULL,
    [sapPersonId]                        NVARCHAR (400)   NULL,
    [sapPosition]                        UNIQUEIDENTIFIER NULL,
    [sapRelationalCategory]              NVARCHAR (400)   NULL,
    [sapStartDate]                       BIGINT           NULL,
    [sn]                                 NVARCHAR (400)   NULL,
    [supervisor]                         UNIQUEIDENTIFIER NULL,
    [supervisorAccountName]              NVARCHAR (400)   NULL,
    [unixAccountName]                    NVARCHAR (400)   NULL,
    [unixGid]                            BIGINT           NULL,
    [unixHomeDirectory]                  NVARCHAR (400)   NULL,
    [unixShell]                          NVARCHAR (400)   NULL,
    [unixUid]                            BIGINT           NULL,
    [usernameFormat]                     NVARCHAR (400)   NULL,
    [zaHRExpiryDate]                     BIGINT           NULL,
    [homeFolderGroup]                    BIGINT           NULL,
    [homeFolderPathProfile]              NVARCHAR (400)   NULL,
    [homeFolderPathSmb]                  NVARCHAR (400)   NULL,
    [homeFolderPathUnc]                  NVARCHAR (400)   NULL,
    [objectSid]                          VARBINARY (800)  NULL,
    [activationToken]                    NVARCHAR (400)   NULL,
    [activationTokenExpiry]              BIGINT           NULL,
    [jmssStudentDBExpiryDate]            BIGINT           NULL,
    [lastActivationDate]                 BIGINT           NULL,
    [manualExpiryDate]                   BIGINT           NULL,
    [newAccountActivationEmailPending]   BIT              NULL,
    [newAccountActivationMail]           NVARCHAR (400)   NULL,
    [newAccountNotificationEmailPending] BIT              NULL,
    [newAccountNotificationMail]         NVARCHAR (400)   NULL,
    [testAttribute]                      NVARCHAR (MAX)   NULL,
    [myAttribute99]                      NVARCHAR (400)   NULL,
    [mytestattribute]                    BIGINT           NULL,
    CONSTRAINT [PK_MA_Objects] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_MA_Objects_MA_SchemaObjects] FOREIGN KEY ([objectClass]) REFERENCES [dbo].[MA_SchemaObjectClasses] ([Name]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_MA_Objects_MA_SchemaShadowObjectLinks] FOREIGN KEY ([shadowLink]) REFERENCES [dbo].[MA_SchemaShadowObjectLinks] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_mvDeleted]
    ON [dbo].[MA_Objects]([deleted] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_accountName]
    ON [dbo].[MA_Objects]([accountName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_adMgrAccount]
    ON [dbo].[MA_Objects]([adMgrAccount] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_callistaPersonIdOverrideRef]
    ON [dbo].[MA_Objects]([callistaPersonIdOverrideRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_shadowParent]
    ON [dbo].[MA_Objects]([shadowParent] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_monashPersonId]
    ON [dbo].[MA_Objects]([monashPersonId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MA_Objects_objectId]
    ON [dbo].[MA_Objects]([objectId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_parentAccount]
    ON [dbo].[MA_Objects]([parentAccount] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_parentPerson]
    ON [dbo].[MA_Objects]([parentPerson] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_sapOrganizationalUnit]
    ON [dbo].[MA_Objects]([sapOrganizationalUnit] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_sapPersonId]
    ON [dbo].[MA_Objects]([sapPersonId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_sapPosition]
    ON [dbo].[MA_Objects]([sapPosition] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_supervisor]
    ON [dbo].[MA_Objects]([supervisor] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_callistaPersonId]
    ON [dbo].[MA_Objects]([callistaPersonId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_displayName]
    ON [dbo].[MA_Objects]([displayName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MA_Objects_sapCentralId]
    ON [dbo].[MA_Objects]([sapCentralId] ASC);


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
