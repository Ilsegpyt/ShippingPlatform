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
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [Drivers] (
        [Id] uniqueidentifier NOT NULL,
        [DriverNumber] nvarchar(80) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [MobileNumber] nvarchar(25) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(max) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [UpdatedByName] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(max) NULL,
        CONSTRAINT [PK_Drivers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [Ports] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(160) NOT NULL,
        [Code] nvarchar(20) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(max) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [UpdatedByName] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(max) NULL,
        CONSTRAINT [PK_Ports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [ShippingLines] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(160) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(max) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [UpdatedByName] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(max) NULL,
        CONSTRAINT [PK_ShippingLines] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [Transporters] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(max) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [UpdatedByName] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(max) NULL,
        CONSTRAINT [PK_Transporters] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [Vehicles] (
        [Id] uniqueidentifier NOT NULL,
        [CarNumber] nvarchar(80) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(max) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [UpdatedByName] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(max) NULL,
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [Operations] (
        [Id] uniqueidentifier NOT NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [ShippingLineId] uniqueidentifier NOT NULL,
        [OperationType] int NOT NULL,
        [Status] int NOT NULL,
        [ShipmentNumber] nvarchar(120) NULL,
        [CertificateNumber] nvarchar(80) NULL,
        [InvoicesReceivedDate] datetime2 NULL,
        [DeliveredAtUtc] datetime2 NULL,
        [DeliveredByUserId] nvarchar(128) NULL,
        [ReopenedAtUtc] datetime2 NULL,
        [ReopenedByUserId] nvarchar(128) NULL,
        [ReopenReason] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(128) NOT NULL,
        [CreatedByName] nvarchar(200) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(128) NULL,
        [UpdatedByName] nvarchar(200) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(200) NULL,
        CONSTRAINT [PK_Operations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Operations_ShippingLines_ShippingLineId] FOREIGN KEY ([ShippingLineId]) REFERENCES [ShippingLines] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [ExportDetails] (
        [Id] uniqueidentifier NOT NULL,
        [OperationId] uniqueidentifier NOT NULL,
        [ClearanceType] int NOT NULL,
        [BookingNumber] nvarchar(120) NOT NULL,
        [POLId] uniqueidentifier NOT NULL,
        [POWId] uniqueidentifier NULL,
        [CutOffDate] date NULL,
        [Customs] nvarchar(160) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(128) NOT NULL,
        [CreatedByName] nvarchar(200) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(128) NULL,
        [UpdatedByName] nvarchar(200) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(200) NULL,
        CONSTRAINT [PK_ExportDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_ExportDetails_ClearanceType] CHECK ([ClearanceType] IN (1, 2)),
        CONSTRAINT [FK_ExportDetails_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExportDetails_Ports_POLId] FOREIGN KEY ([POLId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExportDetails_Ports_POWId] FOREIGN KEY ([POWId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [ImportDetails] (
        [Id] uniqueidentifier NOT NULL,
        [OperationId] uniqueidentifier NOT NULL,
        [MBLNumber] nvarchar(120) NOT NULL,
        [Destination] nvarchar(200) NOT NULL,
        [PODId] uniqueidentifier NOT NULL,
        [FreeTimeTill] date NOT NULL,
        [RequiredOffloadingDate] date NOT NULL,
        [RequiredOffloadingTime] time NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(128) NOT NULL,
        [CreatedByName] nvarchar(200) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(128) NULL,
        [UpdatedByName] nvarchar(200) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(200) NULL,
        CONSTRAINT [PK_ImportDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ImportDetails_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ImportDetails_Ports_PODId] FOREIGN KEY ([PODId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [OperationContainers] (
        [Id] uniqueidentifier NOT NULL,
        [OperationId] uniqueidentifier NOT NULL,
        [Ordinal] smallint NOT NULL,
        [ContainerNumber] nvarchar(11) NULL,
        [ContainerType] int NOT NULL,
        [RequiredLoadingDate] date NULL,
        [RequiredLoadingTime] time NULL,
        [ActualArrivalDate] date NULL,
        [ActualArrivalTime] time NULL,
        [GateOutDate] date NULL,
        [GateOutTime] time NULL,
        [MissingDocs] nvarchar(1000) NULL,
        [MissingDocsReceivedDate] date NULL,
        [TransporterId] uniqueidentifier NULL,
        [DriverId] uniqueidentifier NULL,
        [VehicleId] uniqueidentifier NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(max) NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [UpdatedByName] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [DeletedByName] nvarchar(max) NULL,
        CONSTRAINT [PK_OperationContainers] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_OperationContainer_Ordinal] CHECK ([Ordinal] >= 0 AND [Ordinal] < 200),
        CONSTRAINT [FK_OperationContainers_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationContainers_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationContainers_Transporters_TransporterId] FOREIGN KEY ([TransporterId]) REFERENCES [Transporters] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OperationContainers_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE TABLE [ContainerUpdates] (
        [Id] uniqueidentifier NOT NULL,
        [OperationContainerId] uniqueidentifier NOT NULL,
        [Details] nvarchar(2000) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NOT NULL,
        [CreatedByName] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_ContainerUpdates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ContainerUpdates_OperationContainers_OperationContainerId] FOREIGN KEY ([OperationContainerId]) REFERENCES [OperationContainers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_ContainerUpdates_OperationContainerId] ON [ContainerUpdates] ([OperationContainerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Drivers_DriverNumber] ON [Drivers] ([DriverNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExportDetails_OperationId] ON [ExportDetails] ([OperationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_ExportDetails_POLId] ON [ExportDetails] ([POLId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_ExportDetails_POWId] ON [ExportDetails] ([POWId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ImportDetails_OperationId] ON [ImportDetails] ([OperationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_ImportDetails_PODId] ON [ImportDetails] ([PODId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_OperationContainers_ContainerNumber] ON [OperationContainers] ([ContainerNumber]) WHERE [ContainerNumber] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_OperationContainers_DriverId] ON [OperationContainers] ([DriverId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OperationContainers_OperationId_Ordinal] ON [OperationContainers] ([OperationId], [Ordinal]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_OperationContainers_RequiredLoadingDate_ActualArrivalDate] ON [OperationContainers] ([RequiredLoadingDate], [ActualArrivalDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_OperationContainers_TransporterId] ON [OperationContainers] ([TransporterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_OperationContainers_VehicleId] ON [OperationContainers] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE INDEX [IX_Operations_ShippingLineId] ON [Operations] ([ShippingLineId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Ports_Code] ON [Ports] ([Code]) WHERE [Code] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ShippingLines_Name] ON [ShippingLines] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Transporters_Name] ON [Transporters] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehicles_CarNumber] ON [Vehicles] ([CarNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005081936_InitialOperations'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005081936_InitialOperations', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'CreatedAtUtc');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'CreatedByName');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'CreatedByUserId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeletedAtUtc');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeletedByName');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeletedByUserId');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'IsDeleted');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'UpdatedByName');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'UpdatedByUserId');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [Vehicles] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'CreatedAtUtc');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'CreatedByName');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'CreatedByUserId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'DeletedAtUtc');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'DeletedByName');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'DeletedByUserId');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var16 nvarchar(max);
    SELECT @var16 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'IsDeleted');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var16 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var17 nvarchar(max);
    SELECT @var17 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var17 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var18 nvarchar(max);
    SELECT @var18 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'UpdatedByName');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var18 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var19 nvarchar(max);
    SELECT @var19 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'UpdatedByUserId');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var19 + ';');
    ALTER TABLE [Transporters] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var20 nvarchar(max);
    SELECT @var20 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'CreatedAtUtc');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var20 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var21 nvarchar(max);
    SELECT @var21 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'CreatedByName');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var21 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var22 nvarchar(max);
    SELECT @var22 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'CreatedByUserId');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var22 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var23 nvarchar(max);
    SELECT @var23 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'DeletedAtUtc');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var23 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var24 nvarchar(max);
    SELECT @var24 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'DeletedByName');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var24 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var25 nvarchar(max);
    SELECT @var25 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'DeletedByUserId');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var25 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var26 nvarchar(max);
    SELECT @var26 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'IsDeleted');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var26 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var27 nvarchar(max);
    SELECT @var27 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var27 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var28 nvarchar(max);
    SELECT @var28 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'UpdatedByName');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var28 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var29 nvarchar(max);
    SELECT @var29 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'UpdatedByUserId');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var29 + ';');
    ALTER TABLE [ShippingLines] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var30 nvarchar(max);
    SELECT @var30 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'CreatedAtUtc');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var30 + ';');
    ALTER TABLE [Ports] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var31 nvarchar(max);
    SELECT @var31 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'CreatedByName');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var31 + ';');
    ALTER TABLE [Ports] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var32 nvarchar(max);
    SELECT @var32 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'CreatedByUserId');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var32 + ';');
    ALTER TABLE [Ports] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var33 nvarchar(max);
    SELECT @var33 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'DeletedAtUtc');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var33 + ';');
    ALTER TABLE [Ports] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var34 nvarchar(max);
    SELECT @var34 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'DeletedByName');
    IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var34 + ';');
    ALTER TABLE [Ports] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var35 nvarchar(max);
    SELECT @var35 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'DeletedByUserId');
    IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var35 + ';');
    ALTER TABLE [Ports] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var36 nvarchar(max);
    SELECT @var36 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'IsDeleted');
    IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var36 + ';');
    ALTER TABLE [Ports] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var37 nvarchar(max);
    SELECT @var37 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var37 + ';');
    ALTER TABLE [Ports] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var38 nvarchar(max);
    SELECT @var38 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'UpdatedByName');
    IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var38 + ';');
    ALTER TABLE [Ports] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var39 nvarchar(max);
    SELECT @var39 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'UpdatedByUserId');
    IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var39 + ';');
    ALTER TABLE [Ports] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var40 nvarchar(max);
    SELECT @var40 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'CreatedAtUtc');
    IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var40 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var41 nvarchar(max);
    SELECT @var41 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'CreatedByName');
    IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var41 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var42 nvarchar(max);
    SELECT @var42 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'CreatedByUserId');
    IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var42 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var43 nvarchar(max);
    SELECT @var43 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'DeletedAtUtc');
    IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var43 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var44 nvarchar(max);
    SELECT @var44 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'DeletedByName');
    IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var44 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var45 nvarchar(max);
    SELECT @var45 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'DeletedByUserId');
    IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var45 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var46 nvarchar(max);
    SELECT @var46 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'IsDeleted');
    IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var46 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var47 nvarchar(max);
    SELECT @var47 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var47 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var48 nvarchar(max);
    SELECT @var48 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'UpdatedByName');
    IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var48 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    DECLARE @var49 nvarchar(max);
    SELECT @var49 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'UpdatedByUserId');
    IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var49 + ';');
    ALTER TABLE [Drivers] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005101041_RemoveMasterDataAuditSoftDelete', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    ALTER TABLE [ExportDetails] DROP CONSTRAINT [CK_ExportDetails_ClearanceType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    DECLARE @var50 nvarchar(max);
    SELECT @var50 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'Status');
    IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var50 + ';');
    ALTER TABLE [Operations] ALTER COLUMN [Status] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    DECLARE @var51 nvarchar(max);
    SELECT @var51 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'OperationType');
    IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var51 + ';');
    ALTER TABLE [Operations] ALTER COLUMN [OperationType] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    DECLARE @var52 nvarchar(max);
    SELECT @var52 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'ClearanceType');
    IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var52 + ';');
    ALTER TABLE [ExportDetails] ALTER COLUMN [ClearanceType] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    UPDATE Operations
    SET OperationType =
        CASE OperationType
            WHEN '1' THEN 'Import'
            WHEN '2' THEN 'Export'
            ELSE OperationType
        END;

    UPDATE Operations
    SET Status =
        CASE Status
            WHEN '1' THEN 'Open'
            WHEN '2' THEN 'Delivered'
            ELSE Status
        END;

    UPDATE ExportDetails
    SET ClearanceType =
        CASE ClearanceType
            WHEN '1' THEN 'Bosla'
            WHEN '2' THEN 'Cert'
            ELSE ClearanceType
        END;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    EXEC(N'ALTER TABLE [ExportDetails] ADD CONSTRAINT [CK_ExportDetails_ClearanceType] CHECK ([ClearanceType] IN (''Bosla'', ''Cert''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005114529_StoreOperationEnumsAsStrings', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005123451_AddUpdateImportFields'
)
BEGIN
    ALTER TABLE [OperationContainers] ADD [CheckOutFromThePort] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005123451_AddUpdateImportFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005123451_AddUpdateImportFields', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005131043_AddNumberOfContainers'
)
BEGIN
    ALTER TABLE [Operations] ADD [NumberOfContainers] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005131043_AddNumberOfContainers'
)
BEGIN
    DECLARE @var53 nvarchar(max);
    SELECT @var53 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'ContainerType');
    IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var53 + ';');
    ALTER TABLE [OperationContainers] ALTER COLUMN [ContainerType] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005131043_AddNumberOfContainers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005131043_AddNumberOfContainers', N'10.0.12');
END;

COMMIT;
GO

