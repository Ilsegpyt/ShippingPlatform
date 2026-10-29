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



/* Seed Branches data. Existing IDs are skipped to make this section re-runnable. */
IF OBJECT_ID(N'[dbo].[Branches]', N'U') IS NOT NULL
BEGIN
    SET IDENTITY_INSERT [dbo].[Branches] ON;

    INSERT INTO [dbo].[Branches]
    (
        [Id], [Name], [Address], [Latitude], [Longitude],
        [CeoName], [CeoEmail], [GmName], [GmEmail],
        [IsActive], [CreatedAtUtc], [BranchManagerEmail], [BranchManagerName]
    )
    SELECT
        v.[Id], v.[Name], v.[Address], v.[Latitude], v.[Longitude],
        v.[CeoName], v.[CeoEmail], v.[GmName], v.[GmEmail],
        v.[IsActive], v.[CreatedAtUtc], v.[BranchManagerEmail], v.[BranchManagerName]
    FROM (VALUES
        (2, N'TEDA Office Sokna', N'3rd Floor, Office No. 304 TEDA Building, Suez Economic Zone, Ain El Sokhna, Suez Governorate', CAST(29.6717800 AS decimal(10,7)), CAST(32.3134280 AS decimal(10,7)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(1 AS bit), CAST('2026-10-07T08:56:09.6988445' AS datetime2), N'Mohamed.Alshabssy@ilsegypt.com', N'Mr. Mohamed Alshabssy'),
        (3, N'6th Oct ODP dry port Office', N'ILS | (Egypt) Ltd. | October Dry Port 6th of October City – Wahat Road - Egypt', CAST(29.8222360 AS decimal(10,7)), CAST(30.6699390 AS decimal(10,7)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(1 AS bit), CAST('2026-10-07T08:56:24.9223592' AS datetime2), N'Mohamed.Shalaby@ilsegypt.com', N'Mr. Mohamed Shalaby'),
        (4, N'Sokhna Office', N'ILS | (Egypt) Ltd. | Plot No. 16 A next elwadi village port of Sokhna, Egypt', CAST(29.6664960 AS decimal(10,7)), CAST(32.3328140 AS decimal(10,7)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(1 AS bit), CAST('2026-10-07T08:56:36.5655954' AS datetime2), N'Basem.Fadel@ilsegypt.com', N'Mr. Basem Fadel'),
        (5, N'Alexandria Office', N'ILS | (Egypt) Ltd. Road no.6, Bldg.no.13, from El Moaskar El Romany St. Roushdy, Alexandria, Egypt', CAST(31.2245430 AS decimal(10,7)), CAST(29.9396150 AS decimal(10,7)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(1 AS bit), CAST('2026-10-07T08:56:43.1375954' AS datetime2), N'mohamed.shalaby@ilsegypt.com', N'Mr. Mohamed Shalaby'),
        (6, N'Cairo Airport Office', N'ILS | (Egypt) Ltd. | Cairo Airport, Office 224, CACC Lufthansa old building, opposite to Smash Club', CAST(30.1191610 AS decimal(10,7)), CAST(31.4024850 AS decimal(10,7)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)), CAST(1 AS bit), CAST('2026-10-07T08:56:54.1200360' AS datetime2), N'ahmed.raafat@ilsegypt.com', N'Mr. Ahmed Raafat'),
        (7, N'Head Quarter Office', N'Plot No. 5, Square 1258W, Behind Sun City Mall – Al Nozha – Sheraton – Heliopolis – Cairo, Egypt', CAST(30.1025190 AS decimal(10,7)), CAST(31.3833910 AS decimal(10,7)), N'Mr. Ahmed Shalaby', N'ahmed.shalaby@ilsegypt.com', N'Mr. Karim Raafat', N'karim.raafat@ilsegypt.com', CAST(1 AS bit), CAST('2026-10-07T08:57:28.7576397' AS datetime2), CAST(NULL AS nvarchar(200)), CAST(NULL AS nvarchar(200)))
    ) AS v
    (
        [Id], [Name], [Address], [Latitude], [Longitude],
        [CeoName], [CeoEmail], [GmName], [GmEmail],
        [IsActive], [CreatedAtUtc], [BranchManagerEmail], [BranchManagerName]
    )
    WHERE NOT EXISTS
    (
        SELECT 1 FROM [dbo].[Branches] AS b WHERE b.[Id] = v.[Id]
    );

    SET IDENTITY_INSERT [dbo].[Branches] OFF;
END;
GO
