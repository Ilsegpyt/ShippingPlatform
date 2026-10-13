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
    WHERE [MigrationId] = N'20260831093835_InitialReports'
)
BEGIN
    CREATE TABLE [Reports] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [ShipmentRef] nvarchar(100) NULL,
        [Category] int NOT NULL,
        [Service] int NOT NULL,
        [ShipmentType] int NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [StorageKey] nvarchar(500) NOT NULL,
        [UploadedByUserId] uniqueidentifier NOT NULL,
        [UploadedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Reports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260831093835_InitialReports'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260831093835_InitialReports', N'10.0.12');
END;

COMMIT;
GO

