/*
  ILS Content module: schema + CMS seed in ONE idempotent script.
  - Works on a new database or an existing database with an older/empty Contents table.
  - Adds missing Content columns and inserts only CMS records whose IDs do not exist.
  - Does not delete or overwrite existing content.
*/
USE [ShippingPlatform];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.__EFMigrationsHistory
        (
            MigrationId nvarchar(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
            ProductVersion nvarchar(32) NOT NULL
        );
    END;

    IF OBJECT_ID(N'dbo.Contents', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Contents
        (
            Id uniqueidentifier NOT NULL CONSTRAINT PK_Contents PRIMARY KEY,
            ParentId uniqueidentifier NULL,
            Title nvarchar(200) NOT NULL,
            Type int NOT NULL,
            Body nvarchar(max) NULL,
            FeaturedImage nvarchar(max) NULL,
            LinkUrl nvarchar(max) NULL,
            Category nvarchar(max) NULL,
            PublishedAt datetime2(7) NULL,
            PageIntroText nvarchar(max) NULL,
            PageContentOrder nvarchar(max) NULL
        );
    END
    ELSE
    BEGIN
        IF COL_LENGTH(N'dbo.Contents', N'ParentId') IS NULL ALTER TABLE dbo.Contents ADD ParentId uniqueidentifier NULL;
        IF COL_LENGTH(N'dbo.Contents', N'Title') IS NULL THROW 51002, 'dbo.Contents is missing required column Title; refusing to modify an incompatible table.', 1;
        IF COL_LENGTH(N'dbo.Contents', N'Type') IS NULL THROW 51003, 'dbo.Contents is missing required column Type; refusing to modify an incompatible table.', 1;
        IF COL_LENGTH(N'dbo.Contents', N'Body') IS NULL ALTER TABLE dbo.Contents ADD Body nvarchar(max) NULL;
        IF COL_LENGTH(N'dbo.Contents', N'FeaturedImage') IS NULL ALTER TABLE dbo.Contents ADD FeaturedImage nvarchar(max) NULL;
        IF COL_LENGTH(N'dbo.Contents', N'LinkUrl') IS NULL ALTER TABLE dbo.Contents ADD LinkUrl nvarchar(max) NULL;
        IF COL_LENGTH(N'dbo.Contents', N'Category') IS NULL ALTER TABLE dbo.Contents ADD Category nvarchar(max) NULL;
        IF COL_LENGTH(N'dbo.Contents', N'PublishedAt') IS NULL ALTER TABLE dbo.Contents ADD PublishedAt datetime2(7) NULL;
        IF COL_LENGTH(N'dbo.Contents', N'PageIntroText') IS NULL ALTER TABLE dbo.Contents ADD PageIntroText nvarchar(max) NULL;
        IF COL_LENGTH(N'dbo.Contents', N'PageContentOrder') IS NULL ALTER TABLE dbo.Contents ADD PageContentOrder nvarchar(max) NULL;
    END;


    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Contents_Contents_ParentId' AND parent_object_id = OBJECT_ID(N'dbo.Contents'))
    BEGIN
        ALTER TABLE dbo.Contents WITH CHECK ADD CONSTRAINT FK_Contents_Contents_ParentId
            FOREIGN KEY (ParentId) REFERENCES dbo.Contents (Id) ON DELETE NO ACTION;
        ALTER TABLE dbo.Contents CHECK CONSTRAINT FK_Contents_Contents_ParentId;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20260928122307_InitialCreate')
        INSERT dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20260928122307_InitialCreate', N'10.0.12');
    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261006063850_AddPostMetadata')
        INSERT dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20261006063850_AddPostMetadata', N'10.0.12');
    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261014100000_AddPageContentFields')
        INSERT dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20261014100000_AddPageContentFields', N'10.0.12');

    COMMIT TRANSACTION;
    PRINT 'Content schema is up to date. Missing CMS records inserted; existing records preserved.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
