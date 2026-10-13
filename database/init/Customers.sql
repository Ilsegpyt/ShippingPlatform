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
    WHERE [MigrationId] = N'20260819063922_InitialCustomers'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] uniqueidentifier NOT NULL,
        [OwnerName] nvarchar(200) NOT NULL,
        [CompanyName] nvarchar(200) NOT NULL,
        [OwnerPhone] nvarchar(30) NOT NULL,
        [OwnerEmail] nvarchar(256) NOT NULL,
        [Industry] nvarchar(200) NULL,
        [OwnerUserId] uniqueidentifier NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819063922_InitialCustomers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_OwnerUserId] ON [Customers] ([OwnerUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819063922_InitialCustomers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819063922_InitialCustomers', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901131504_AddSearchHistories'
)
BEGIN
    CREATE TABLE [SearchHistories] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [Origin] nvarchar(100) NOT NULL,
        [Destination] nvarchar(100) NOT NULL,
        [ContainerSize] nvarchar(50) NOT NULL,
        [DepartureDate] date NOT NULL,
        [RoutesFound] int NOT NULL,
        [SearchedOnUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_SearchHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SearchHistories_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901131504_AddSearchHistories'
)
BEGIN
    CREATE INDEX [IX_SearchHistories_CustomerId] ON [SearchHistories] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901131504_AddSearchHistories'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260901131504_AddSearchHistories', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923065917_AddCustomerVoices'
)
BEGIN
    CREATE TABLE [CustomerVoices] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [ShipmentId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Subject] nvarchar(200) NOT NULL,
        [Message] nvarchar(4000) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_CustomerVoices] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923065917_AddCustomerVoices'
)
BEGIN
    CREATE INDEX [IX_CustomerVoices_CustomerId] ON [CustomerVoices] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923065917_AddCustomerVoices'
)
BEGIN
    CREATE INDEX [IX_CustomerVoices_ShipmentId] ON [CustomerVoices] ([ShipmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923065917_AddCustomerVoices'
)
BEGIN
    CREATE INDEX [IX_CustomerVoices_UserId] ON [CustomerVoices] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923065917_AddCustomerVoices'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260923065917_AddCustomerVoices', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923092103_StoreCustomerVoiceStatusAsString'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CustomerVoices]') AND [c].[name] = N'Status');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [CustomerVoices] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [CustomerVoices] ALTER COLUMN [Status] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923092103_StoreCustomerVoiceStatusAsString'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260923092103_StoreCustomerVoiceStatusAsString', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005102000_AddDeletedByNameToCustomers'
)
BEGIN
    ALTER TABLE [Customers] ADD [DeletedByName] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005102000_AddDeletedByNameToCustomers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005102000_AddDeletedByNameToCustomers', N'10.0.12');
END;

COMMIT;
GO

