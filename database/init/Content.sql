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
    WHERE [MigrationId] = N'20260928122307_InitialCreate'
)
BEGIN
    CREATE TABLE [Contents] (
        [Id] uniqueidentifier NOT NULL,
        [ParentId] uniqueidentifier NULL,
        [Title] nvarchar(200) NOT NULL,
        [Type] int NOT NULL,
        [Body] nvarchar(max) NULL,
        [FeaturedImage] nvarchar(max) NULL,
        [LinkUrl] nvarchar(max) NULL,
        CONSTRAINT [PK_Contents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contents_Contents_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Contents] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928122307_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Contents_ParentId] ON [Contents] ([ParentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928122307_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928122307_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006063850_AddPostMetadata'
)
BEGIN
    ALTER TABLE [Contents] ADD [Category] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006063850_AddPostMetadata'
)
BEGIN
    ALTER TABLE [Contents] ADD [PublishedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006063850_AddPostMetadata'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006063850_AddPostMetadata', N'10.0.12');
END;

COMMIT;
GO

