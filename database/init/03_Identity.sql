SET QUOTED_IDENTIFIER ON;
GO



IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [AccountManagerAssignments] (
        [Id] uniqueidentifier NOT NULL,
        [AccountManagerUserId] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [AssignedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AccountManagerAssignments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [IdentityRoles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_IdentityRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [ImpersonationAuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [ImpersonatorUserId] uniqueidentifier NOT NULL,
        [TargetCustomerUserId] uniqueidentifier NOT NULL,
        [StartedAtUtc] datetime2 NOT NULL,
        [EndedAtUtc] datetime2 NULL,
        [IpAddress] nvarchar(max) NULL,
        CONSTRAINT [PK_ImpersonationAuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [InternalUsers] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_InternalUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(200) NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [RevokedAtUtc] datetime2 NULL,
        [ReplacedByTokenId] uniqueidentifier NULL,
        [CreatedByIp] nvarchar(max) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [SubAccounts] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [GrantType] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_SubAccounts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [Kind] int NOT NULL,
        [IsActive] bit NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_IdentityRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [IdentityRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [Id] int NOT NULL IDENTITY,
        [PermissionKey] nvarchar(100) NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [SubAccountPermissions] (
        [Id] int NOT NULL IDENTITY,
        [PermissionKey] nvarchar(100) NOT NULL,
        [SubAccountId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SubAccountPermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubAccountPermissions_SubAccounts_SubAccountId] FOREIGN KEY ([SubAccountId]) REFERENCES [SubAccounts] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [SubAccountScopes] (
        [Id] int NOT NULL IDENTITY,
        [Category] nvarchar(20) NOT NULL,
        [Service] nvarchar(20) NOT NULL,
        [Type] nvarchar(20) NOT NULL,
        [SubAccountId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SubAccountScopes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubAccountScopes_SubAccounts_SubAccountId] FOREIGN KEY ([SubAccountId]) REFERENCES [SubAccounts] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_IdentityRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [IdentityRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AccountManagerAssignments_AccountManagerUserId_CustomerId] ON [AccountManagerAssignments] ([AccountManagerUserId], [CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [IdentityRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ImpersonationAuditLogs_ImpersonatorUserId] ON [ImpersonationAuditLogs] ([ImpersonatorUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InternalUsers_RoleId] ON [InternalUsers] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InternalUsers_UserId] ON [InternalUsers] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_RoleId] ON [RolePermissions] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SubAccountPermissions_SubAccountId] ON [SubAccountPermissions] ([SubAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SubAccounts_OrganizationId] ON [SubAccounts] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SubAccounts_UserId] ON [SubAccounts] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SubAccountScopes_SubAccountId] ON [SubAccountScopes] ([SubAccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260811114318_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260811114318_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260816062715_AddNameToSubAccount'
)
BEGIN
    ALTER TABLE [SubAccounts] ADD [Name] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260816062715_AddNameToSubAccount'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260816062715_AddNameToSubAccount', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260822121658_RenameGrantTypeToScopeType'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RefreshTokens]') AND [c].[name] = N'CreatedByIp');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [RefreshTokens] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [RefreshTokens] DROP COLUMN [CreatedByIp];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260822121658_RenameGrantTypeToScopeType'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImpersonationAuditLogs]') AND [c].[name] = N'IpAddress');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ImpersonationAuditLogs] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [ImpersonationAuditLogs] DROP COLUMN [IpAddress];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260822121658_RenameGrantTypeToScopeType'
)
BEGIN
    EXEC sp_rename N'[SubAccounts].[GrantType]', N'ScopeType', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260822121658_RenameGrantTypeToScopeType'
)
BEGIN
    CREATE INDEX [IX_ImpersonationAuditLogs_TargetCustomerUserId] ON [ImpersonationAuditLogs] ([TargetCustomerUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260822121658_RenameGrantTypeToScopeType'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260822121658_RenameGrantTypeToScopeType', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824115741_AddEmailToSubAccounts'
)
BEGIN
    ALTER TABLE [SubAccounts] ADD [Email] nvarchar(320) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824115741_AddEmailToSubAccounts'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260824115741_AddEmailToSubAccounts', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825113302_AddEmailAndPhoneToInternalUsers'
)
BEGIN
    ALTER TABLE [InternalUsers] ADD [Email] nvarchar(256) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825113302_AddEmailAndPhoneToInternalUsers'
)
BEGIN
    ALTER TABLE [InternalUsers] ADD [Phone] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825113302_AddEmailAndPhoneToInternalUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260825113302_AddEmailAndPhoneToInternalUsers', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826055535_AddNameToInternalUsers'
)
BEGIN
    ALTER TABLE [InternalUsers] ADD [Name] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826055535_AddNameToInternalUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260826055535_AddNameToInternalUsers', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830080201_AddOutboxMessages'
)
BEGIN
    CREATE TABLE [OutboxMessages] (
        [Id] uniqueidentifier NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [Payload] nvarchar(max) NOT NULL,
        [OccurredOnUtc] datetime2 NOT NULL,
        [ProcessedOnUtc] datetime2 NULL,
        [Error] nvarchar(max) NULL,
        CONSTRAINT [PK_OutboxMessages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830080201_AddOutboxMessages'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260830080201_AddOutboxMessages', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830092623_AddOutboxRetryCount'
)
BEGIN
    ALTER TABLE [OutboxMessages] ADD [RetryCount] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830092623_AddOutboxRetryCount'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260830092623_AddOutboxRetryCount', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830133951_AddAccountManagerAssignments'
)
BEGIN
    DROP INDEX [IX_AccountManagerAssignments_AccountManagerUserId_CustomerId] ON [AccountManagerAssignments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830133951_AddAccountManagerAssignments'
)
BEGIN
    EXEC sp_rename N'[AccountManagerAssignments].[AccountManagerUserId]', N'AccountManagerId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830133951_AddAccountManagerAssignments'
)
BEGIN
    CREATE INDEX [IX_AccountManagerAssignments_AccountManagerId] ON [AccountManagerAssignments] ([AccountManagerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830133951_AddAccountManagerAssignments'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AccountManagerAssignments_CustomerId] ON [AccountManagerAssignments] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260830133951_AddAccountManagerAssignments'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260830133951_AddAccountManagerAssignments', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902083439_UpdateImpersonationAuditLogs'
)
BEGIN
    ALTER TABLE [ImpersonationAuditLogs] ADD [IpAddress] nvarchar(45) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902083439_UpdateImpersonationAuditLogs'
)
BEGIN
    ALTER TABLE [ImpersonationAuditLogs] ADD [Reason] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902083439_UpdateImpersonationAuditLogs'
)
BEGIN
    ALTER TABLE [ImpersonationAuditLogs] ADD [TokenId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902083439_UpdateImpersonationAuditLogs'
)
BEGIN
    ALTER TABLE [ImpersonationAuditLogs] ADD [UserAgent] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902083439_UpdateImpersonationAuditLogs'
)
BEGIN
    CREATE INDEX [IX_ImpersonationAuditLogs_StartedAtUtc] ON [ImpersonationAuditLogs] ([StartedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902083439_UpdateImpersonationAuditLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260902083439_UpdateImpersonationAuditLogs', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902113949_AddImpersonationFieldsToRefreshTokens'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImpersonationAuditLogs]') AND [c].[name] = N'TokenId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ImpersonationAuditLogs] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [ImpersonationAuditLogs] DROP COLUMN [TokenId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902113949_AddImpersonationFieldsToRefreshTokens'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [ImpersonatedOrganizationId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902113949_AddImpersonationFieldsToRefreshTokens'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [TokenType] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902113949_AddImpersonationFieldsToRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_ImpersonatedOrganizationId] ON [RefreshTokens] ([ImpersonatedOrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260902113949_AddImpersonationFieldsToRefreshTokens'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260902113949_AddImpersonationFieldsToRefreshTokens', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910074134_RenameOrganizationIdsToCustomerIds'
)
BEGIN
    EXEC sp_rename N'[SubAccounts].[OrganizationId]', N'CustomerId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910074134_RenameOrganizationIdsToCustomerIds'
)
BEGIN
    EXEC sp_rename N'[SubAccounts].[IX_SubAccounts_OrganizationId]', N'IX_SubAccounts_CustomerId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910074134_RenameOrganizationIdsToCustomerIds'
)
BEGIN
    EXEC sp_rename N'[RefreshTokens].[ImpersonatedOrganizationId]', N'ImpersonatedCustomerId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910074134_RenameOrganizationIdsToCustomerIds'
)
BEGIN
    EXEC sp_rename N'[RefreshTokens].[IX_RefreshTokens_ImpersonatedOrganizationId]', N'IX_RefreshTokens_ImpersonatedCustomerId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910074134_RenameOrganizationIdsToCustomerIds'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260910074134_RenameOrganizationIdsToCustomerIds', N'10.0.12');
END;

COMMIT;
GO

