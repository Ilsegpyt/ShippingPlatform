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
    WHERE [MigrationId] = N'20261007063706_InitialWebsite'
)
BEGIN
    CREATE TABLE [AgentApplications] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Country] nvarchar(100) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [Address] nvarchar(300) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [CountryCode] nvarchar(10) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [MainIndustry] nvarchar(200) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AgentApplications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007063706_InitialWebsite'
)
BEGIN
    CREATE TABLE [Branches] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Address] nvarchar(1000) NOT NULL,
        [Latitude] decimal(10,7) NOT NULL,
        [Longitude] decimal(10,7) NOT NULL,
        [CeoName] nvarchar(200) NOT NULL,
        [CeoEmail] nvarchar(200) NOT NULL,
        [GmName] nvarchar(200) NOT NULL,
        [GmEmail] nvarchar(200) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Branches] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007063706_InitialWebsite'
)
BEGIN
    CREATE TABLE [ContactInquiries] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(200) NOT NULL,
        [Company] nvarchar(200) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [Service] nvarchar(200) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_ContactInquiries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007063706_InitialWebsite'
)
BEGIN
    CREATE TABLE [QuoteRequests] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Company] nvarchar(200) NOT NULL,
        [Country] nvarchar(100) NOT NULL,
        [CountryCode] nvarchar(10) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [InterestType] nvarchar(20) NOT NULL,
        [TransportMode] nvarchar(20) NOT NULL,
        [AnnualShipments] nvarchar(30) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_QuoteRequests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007063706_InitialWebsite'
)
BEGIN
    CREATE TABLE [RecruitmentApplications] (
        [Id] int NOT NULL IDENTITY,
        [Department] nvarchar(150) NOT NULL,
        [CvFileName] nvarchar(255) NOT NULL,
        [CvFilePath] nvarchar(500) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_RecruitmentApplications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007063706_InitialWebsite'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007063706_InitialWebsite', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084148_AddBranchManagerFields'
)
BEGIN
    ALTER TABLE [Branches] ADD [BranchManagerEmail] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084148_AddBranchManagerFields'
)
BEGIN
    ALTER TABLE [Branches] ADD [BranchManagerName] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084148_AddBranchManagerFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007084148_AddBranchManagerFields', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'GmName');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Branches] ALTER COLUMN [GmName] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'GmEmail');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Branches] ALTER COLUMN [GmEmail] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'CeoName');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Branches] ALTER COLUMN [CeoName] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'CeoEmail');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Branches] ALTER COLUMN [CeoEmail] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'BranchManagerName');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Branches] ALTER COLUMN [BranchManagerName] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'BranchManagerEmail');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Branches] ALTER COLUMN [BranchManagerEmail] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007084700_MakeBranchContactsOptional'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007084700_MakeBranchContactsOptional', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007112405_UpdateQuoteRequestTransportModes'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[QuoteRequests]') AND [c].[name] = N'TransportMode');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [QuoteRequests] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [QuoteRequests] DROP COLUMN [TransportMode];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007112405_UpdateQuoteRequestTransportModes'
)
BEGIN
    ALTER TABLE [QuoteRequests] ADD [TransportModes] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007112405_UpdateQuoteRequestTransportModes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007112405_UpdateQuoteRequestTransportModes', N'10.0.12');
END;

COMMIT;
GO

