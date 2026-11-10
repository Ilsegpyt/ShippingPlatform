SET QUOTED_IDENTIFIER ON;
GO
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL BEGIN     CREATE TABLE [__EFMigrationsHistory] (         [MigrationId] nvarchar(150) NOT NULL,         [ProductVersion] nvarchar(32) NOT NULL,         CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])     ); END;
GO
BEGIN TRANSACTION; IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [Drivers] (         [Id] uniqueidentifier NOT NULL,         [DriverNumber] nvarchar(80) NOT NULL,         [Name] nvarchar(200) NOT NULL,         [MobileNumber] nvarchar(25) NULL,         [IsActive] bit NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(max) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(max) NULL,         [UpdatedByName] nvarchar(max) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(max) NULL,         CONSTRAINT [PK_Drivers] PRIMARY KEY ([Id])     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [Ports] (         [Id] uniqueidentifier NOT NULL,         [Name] nvarchar(160) NOT NULL,         [Code] nvarchar(20) NULL,         [IsActive] bit NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(max) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(max) NULL,         [UpdatedByName] nvarchar(max) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(max) NULL,         CONSTRAINT [PK_Ports] PRIMARY KEY ([Id])     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [ShippingLines] (         [Id] uniqueidentifier NOT NULL,         [Name] nvarchar(160) NOT NULL,         [IsActive] bit NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(max) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(max) NULL,         [UpdatedByName] nvarchar(max) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(max) NULL,         CONSTRAINT [PK_ShippingLines] PRIMARY KEY ([Id])     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [Transporters] (         [Id] uniqueidentifier NOT NULL,         [Name] nvarchar(200) NOT NULL,         [IsActive] bit NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(max) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(max) NULL,         [UpdatedByName] nvarchar(max) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(max) NULL,         CONSTRAINT [PK_Transporters] PRIMARY KEY ([Id])     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [Vehicles] (         [Id] uniqueidentifier NOT NULL,         [CarNumber] nvarchar(80) NOT NULL,         [IsActive] bit NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(max) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(max) NULL,         [UpdatedByName] nvarchar(max) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(max) NULL,         CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id])     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [Operations] (         [Id] uniqueidentifier NOT NULL,         [ClientId] uniqueidentifier NOT NULL,         [ShippingLineId] uniqueidentifier NOT NULL,         [OperationType] int NOT NULL,         [Status] int NOT NULL,         [ShipmentNumber] nvarchar(120) NULL,         [CertificateNumber] nvarchar(80) NULL,         [InvoicesReceivedDate] datetime2 NULL,         [DeliveredAtUtc] datetime2 NULL,         [DeliveredByUserId] nvarchar(128) NULL,         [ReopenedAtUtc] datetime2 NULL,         [ReopenedByUserId] nvarchar(128) NULL,         [ReopenReason] nvarchar(500) NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(128) NOT NULL,         [CreatedByName] nvarchar(200) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(128) NULL,         [UpdatedByName] nvarchar(200) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(200) NULL,         CONSTRAINT [PK_Operations] PRIMARY KEY ([Id]),         CONSTRAINT [FK_Operations_ShippingLines_ShippingLineId] FOREIGN KEY ([ShippingLineId]) REFERENCES [ShippingLines] ([Id]) ON DELETE NO ACTION     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [ExportDetails] (         [Id] uniqueidentifier NOT NULL,         [OperationId] uniqueidentifier NOT NULL,         [ClearanceType] int NOT NULL,         [BookingNumber] nvarchar(120) NOT NULL,         [POLId] uniqueidentifier NOT NULL,         [POWId] uniqueidentifier NULL,         [CutOffDate] date NULL,         [Customs] nvarchar(160) NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(128) NOT NULL,         [CreatedByName] nvarchar(200) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(128) NULL,         [UpdatedByName] nvarchar(200) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(200) NULL,         CONSTRAINT [PK_ExportDetails] PRIMARY KEY ([Id]),         CONSTRAINT [CK_ExportDetails_ClearanceType] CHECK ([ClearanceType] IN (1, 2)),         CONSTRAINT [FK_ExportDetails_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,         CONSTRAINT [FK_ExportDetails_Ports_POLId] FOREIGN KEY ([POLId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION,         CONSTRAINT [FK_ExportDetails_Ports_POWId] FOREIGN KEY ([POWId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [ImportDetails] (         [Id] uniqueidentifier NOT NULL,         [OperationId] uniqueidentifier NOT NULL,         [MBLNumber] nvarchar(120) NOT NULL,         [Destination] nvarchar(200) NOT NULL,         [PODId] uniqueidentifier NOT NULL,         [FreeTimeTill] date NOT NULL,         [RequiredOffloadingDate] date NOT NULL,         [RequiredOffloadingTime] time NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(128) NOT NULL,         [CreatedByName] nvarchar(200) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(128) NULL,         [UpdatedByName] nvarchar(200) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(200) NULL,         CONSTRAINT [PK_ImportDetails] PRIMARY KEY ([Id]),         CONSTRAINT [FK_ImportDetails_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,         CONSTRAINT [FK_ImportDetails_Ports_PODId] FOREIGN KEY ([PODId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [OperationContainers] (         [Id] uniqueidentifier NOT NULL,         [OperationId] uniqueidentifier NOT NULL,         [Ordinal] smallint NOT NULL,         [ContainerNumber] nvarchar(11) NULL,         [ContainerType] int NOT NULL,         [RequiredLoadingDate] date NULL,         [RequiredLoadingTime] time NULL,         [ActualArrivalDate] date NULL,         [ActualArrivalTime] time NULL,         [GateOutDate] date NULL,         [GateOutTime] time NULL,         [MissingDocs] nvarchar(1000) NULL,         [MissingDocsReceivedDate] date NULL,         [TransporterId] uniqueidentifier NULL,         [DriverId] uniqueidentifier NULL,         [VehicleId] uniqueidentifier NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(max) NOT NULL,         [UpdatedAtUtc] datetime2 NULL,         [UpdatedByUserId] nvarchar(max) NULL,         [UpdatedByName] nvarchar(max) NULL,         [IsDeleted] bit NOT NULL,         [DeletedAtUtc] datetime2 NULL,         [DeletedByUserId] uniqueidentifier NULL,         [DeletedByName] nvarchar(max) NULL,         CONSTRAINT [PK_OperationContainers] PRIMARY KEY ([Id]),         CONSTRAINT [CK_OperationContainer_Ordinal] CHECK ([Ordinal] >= 0 AND [Ordinal] < 200),         CONSTRAINT [FK_OperationContainers_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]) ON DELETE NO ACTION,         CONSTRAINT [FK_OperationContainers_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,         CONSTRAINT [FK_OperationContainers_Transporters_TransporterId] FOREIGN KEY ([TransporterId]) REFERENCES [Transporters] ([Id]) ON DELETE NO ACTION,         CONSTRAINT [FK_OperationContainers_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE TABLE [ContainerUpdates] (         [Id] uniqueidentifier NOT NULL,         [OperationContainerId] uniqueidentifier NOT NULL,         [Details] nvarchar(2000) NOT NULL,         [CreatedAtUtc] datetime2 NOT NULL,         [CreatedByUserId] nvarchar(max) NOT NULL,         [CreatedByName] nvarchar(200) NOT NULL,         CONSTRAINT [PK_ContainerUpdates] PRIMARY KEY ([Id]),         CONSTRAINT [FK_ContainerUpdates_OperationContainers_OperationContainerId] FOREIGN KEY ([OperationContainerId]) REFERENCES [OperationContainers] ([Id]) ON DELETE NO ACTION     ); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_ContainerUpdates_OperationContainerId] ON [ContainerUpdates] ([OperationContainerId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_Drivers_DriverNumber] ON [Drivers] ([DriverNumber]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_ExportDetails_OperationId] ON [ExportDetails] ([OperationId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_ExportDetails_POLId] ON [ExportDetails] ([POLId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_ExportDetails_POWId] ON [ExportDetails] ([POWId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_ImportDetails_OperationId] ON [ImportDetails] ([OperationId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_ImportDetails_PODId] ON [ImportDetails] ([PODId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     EXEC(N'CREATE INDEX [IX_OperationContainers_ContainerNumber] ON [OperationContainers] ([ContainerNumber]) WHERE [ContainerNumber] IS NOT NULL'); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_OperationContainers_DriverId] ON [OperationContainers] ([DriverId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_OperationContainers_OperationId_Ordinal] ON [OperationContainers] ([OperationId], [Ordinal]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_OperationContainers_RequiredLoadingDate_ActualArrivalDate] ON [OperationContainers] ([RequiredLoadingDate], [ActualArrivalDate]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_OperationContainers_TransporterId] ON [OperationContainers] ([TransporterId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_OperationContainers_VehicleId] ON [OperationContainers] ([VehicleId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE INDEX [IX_Operations_ShippingLineId] ON [Operations] ([ShippingLineId]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     EXEC(N'CREATE UNIQUE INDEX [IX_Ports_Code] ON [Ports] ([Code]) WHERE [Code] IS NOT NULL'); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_ShippingLines_Name] ON [ShippingLines] ([Name]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_Transporters_Name] ON [Transporters] ([Name]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     CREATE UNIQUE INDEX [IX_Vehicles_CarNumber] ON [Vehicles] ([CarNumber]); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005081936_InitialOperations' ) BEGIN     INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])     VALUES (N'20261005081936_InitialOperations', N'10.0.12'); END;  COMMIT;
GO
BEGIN TRANSACTION; IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var nvarchar(max);     SELECT @var = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'CreatedAtUtc');     IF @var IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var + ';');     ALTER TABLE [Vehicles] DROP COLUMN [CreatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var1 nvarchar(max);     SELECT @var1 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'CreatedByName');     IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var1 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [CreatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var2 nvarchar(max);     SELECT @var2 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'CreatedByUserId');     IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var2 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [CreatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var3 nvarchar(max);     SELECT @var3 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeletedAtUtc');     IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var3 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [DeletedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var4 nvarchar(max);     SELECT @var4 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeletedByName');     IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var4 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [DeletedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var5 nvarchar(max);     SELECT @var5 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeletedByUserId');     IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var5 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [DeletedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var6 nvarchar(max);     SELECT @var6 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'IsDeleted');     IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var6 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [IsDeleted]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var7 nvarchar(max);     SELECT @var7 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'UpdatedAtUtc');     IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var7 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [UpdatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var8 nvarchar(max);     SELECT @var8 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'UpdatedByName');     IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var8 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [UpdatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var9 nvarchar(max);     SELECT @var9 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'UpdatedByUserId');     IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT ' + @var9 + ';');     ALTER TABLE [Vehicles] DROP COLUMN [UpdatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var10 nvarchar(max);     SELECT @var10 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'CreatedAtUtc');     IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var10 + ';');     ALTER TABLE [Transporters] DROP COLUMN [CreatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var11 nvarchar(max);     SELECT @var11 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'CreatedByName');     IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var11 + ';');     ALTER TABLE [Transporters] DROP COLUMN [CreatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var12 nvarchar(max);     SELECT @var12 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'CreatedByUserId');     IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var12 + ';');     ALTER TABLE [Transporters] DROP COLUMN [CreatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var13 nvarchar(max);     SELECT @var13 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'DeletedAtUtc');     IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var13 + ';');     ALTER TABLE [Transporters] DROP COLUMN [DeletedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var14 nvarchar(max);     SELECT @var14 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'DeletedByName');     IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var14 + ';');     ALTER TABLE [Transporters] DROP COLUMN [DeletedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var15 nvarchar(max);     SELECT @var15 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'DeletedByUserId');     IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var15 + ';');     ALTER TABLE [Transporters] DROP COLUMN [DeletedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var16 nvarchar(max);     SELECT @var16 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'IsDeleted');     IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var16 + ';');     ALTER TABLE [Transporters] DROP COLUMN [IsDeleted]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var17 nvarchar(max);     SELECT @var17 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'UpdatedAtUtc');     IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var17 + ';');     ALTER TABLE [Transporters] DROP COLUMN [UpdatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var18 nvarchar(max);     SELECT @var18 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'UpdatedByName');     IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var18 + ';');     ALTER TABLE [Transporters] DROP COLUMN [UpdatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var19 nvarchar(max);     SELECT @var19 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transporters]') AND [c].[name] = N'UpdatedByUserId');     IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Transporters] DROP CONSTRAINT ' + @var19 + ';');     ALTER TABLE [Transporters] DROP COLUMN [UpdatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var20 nvarchar(max);     SELECT @var20 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'CreatedAtUtc');     IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var20 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [CreatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var21 nvarchar(max);     SELECT @var21 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'CreatedByName');     IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var21 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [CreatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var22 nvarchar(max);     SELECT @var22 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'CreatedByUserId');     IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var22 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [CreatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var23 nvarchar(max);     SELECT @var23 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'DeletedAtUtc');     IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var23 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [DeletedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var24 nvarchar(max);     SELECT @var24 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'DeletedByName');     IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var24 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [DeletedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var25 nvarchar(max);     SELECT @var25 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'DeletedByUserId');     IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var25 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [DeletedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var26 nvarchar(max);     SELECT @var26 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'IsDeleted');     IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var26 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [IsDeleted]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var27 nvarchar(max);     SELECT @var27 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'UpdatedAtUtc');     IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var27 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [UpdatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var28 nvarchar(max);     SELECT @var28 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'UpdatedByName');     IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var28 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [UpdatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var29 nvarchar(max);     SELECT @var29 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShippingLines]') AND [c].[name] = N'UpdatedByUserId');     IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [ShippingLines] DROP CONSTRAINT ' + @var29 + ';');     ALTER TABLE [ShippingLines] DROP COLUMN [UpdatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var30 nvarchar(max);     SELECT @var30 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'CreatedAtUtc');     IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var30 + ';');     ALTER TABLE [Ports] DROP COLUMN [CreatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var31 nvarchar(max);     SELECT @var31 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'CreatedByName');     IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var31 + ';');     ALTER TABLE [Ports] DROP COLUMN [CreatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var32 nvarchar(max);     SELECT @var32 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'CreatedByUserId');     IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var32 + ';');     ALTER TABLE [Ports] DROP COLUMN [CreatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var33 nvarchar(max);     SELECT @var33 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'DeletedAtUtc');     IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var33 + ';');     ALTER TABLE [Ports] DROP COLUMN [DeletedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var34 nvarchar(max);     SELECT @var34 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'DeletedByName');     IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var34 + ';');     ALTER TABLE [Ports] DROP COLUMN [DeletedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var35 nvarchar(max);     SELECT @var35 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'DeletedByUserId');     IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var35 + ';');     ALTER TABLE [Ports] DROP COLUMN [DeletedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var36 nvarchar(max);     SELECT @var36 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'IsDeleted');     IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var36 + ';');     ALTER TABLE [Ports] DROP COLUMN [IsDeleted]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var37 nvarchar(max);     SELECT @var37 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'UpdatedAtUtc');     IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var37 + ';');     ALTER TABLE [Ports] DROP COLUMN [UpdatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var38 nvarchar(max);     SELECT @var38 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'UpdatedByName');     IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var38 + ';');     ALTER TABLE [Ports] DROP COLUMN [UpdatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var39 nvarchar(max);     SELECT @var39 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ports]') AND [c].[name] = N'UpdatedByUserId');     IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Ports] DROP CONSTRAINT ' + @var39 + ';');     ALTER TABLE [Ports] DROP COLUMN [UpdatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var40 nvarchar(max);     SELECT @var40 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'CreatedAtUtc');     IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var40 + ';');     ALTER TABLE [Drivers] DROP COLUMN [CreatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var41 nvarchar(max);     SELECT @var41 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'CreatedByName');     IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var41 + ';');     ALTER TABLE [Drivers] DROP COLUMN [CreatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var42 nvarchar(max);     SELECT @var42 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'CreatedByUserId');     IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var42 + ';');     ALTER TABLE [Drivers] DROP COLUMN [CreatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var43 nvarchar(max);     SELECT @var43 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'DeletedAtUtc');     IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var43 + ';');     ALTER TABLE [Drivers] DROP COLUMN [DeletedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var44 nvarchar(max);     SELECT @var44 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'DeletedByName');     IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var44 + ';');     ALTER TABLE [Drivers] DROP COLUMN [DeletedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var45 nvarchar(max);     SELECT @var45 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'DeletedByUserId');     IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var45 + ';');     ALTER TABLE [Drivers] DROP COLUMN [DeletedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var46 nvarchar(max);     SELECT @var46 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'IsDeleted');     IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var46 + ';');     ALTER TABLE [Drivers] DROP COLUMN [IsDeleted]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var47 nvarchar(max);     SELECT @var47 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'UpdatedAtUtc');     IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var47 + ';');     ALTER TABLE [Drivers] DROP COLUMN [UpdatedAtUtc]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var48 nvarchar(max);     SELECT @var48 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'UpdatedByName');     IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var48 + ';');     ALTER TABLE [Drivers] DROP COLUMN [UpdatedByName]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     DECLARE @var49 nvarchar(max);     SELECT @var49 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'UpdatedByUserId');     IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT ' + @var49 + ';');     ALTER TABLE [Drivers] DROP COLUMN [UpdatedByUserId]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005101041_RemoveMasterDataAuditSoftDelete' ) BEGIN     INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])     VALUES (N'20261005101041_RemoveMasterDataAuditSoftDelete', N'10.0.12'); END;  COMMIT;
GO
BEGIN TRANSACTION; IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     ALTER TABLE [ExportDetails] DROP CONSTRAINT [CK_ExportDetails_ClearanceType]; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     DECLARE @var50 nvarchar(max);     SELECT @var50 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'Status');     IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var50 + ';');     ALTER TABLE [Operations] ALTER COLUMN [Status] nvarchar(20) NOT NULL; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     DECLARE @var51 nvarchar(max);     SELECT @var51 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'OperationType');     IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var51 + ';');     ALTER TABLE [Operations] ALTER COLUMN [OperationType] nvarchar(20) NOT NULL; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     DECLARE @var52 nvarchar(max);     SELECT @var52 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'ClearanceType');     IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var52 + ';');     ALTER TABLE [ExportDetails] ALTER COLUMN [ClearanceType] nvarchar(20) NOT NULL; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     UPDATE Operations     SET OperationType =         CASE OperationType             WHEN '1' THEN 'Import'             WHEN '2' THEN 'Export'             ELSE OperationType         END;      UPDATE Operations     SET Status =         CASE Status             WHEN '1' THEN 'Open'             WHEN '2' THEN 'Delivered'             ELSE Status         END;      UPDATE ExportDetails     SET ClearanceType =         CASE ClearanceType             WHEN '1' THEN 'Bosla'             WHEN '2' THEN 'Cert'             ELSE ClearanceType         END; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     EXEC(N'ALTER TABLE [ExportDetails] ADD CONSTRAINT [CK_ExportDetails_ClearanceType] CHECK ([ClearanceType] IN (''Bosla'', ''Cert''))'); END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005114529_StoreOperationEnumsAsStrings' ) BEGIN     INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])     VALUES (N'20261005114529_StoreOperationEnumsAsStrings', N'10.0.12'); END;  COMMIT;
GO
BEGIN TRANSACTION; IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005123451_AddUpdateImportFields' ) BEGIN     ALTER TABLE [OperationContainers] ADD [CheckOutFromThePort] datetime2 NULL; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005123451_AddUpdateImportFields' ) BEGIN     INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])     VALUES (N'20261005123451_AddUpdateImportFields', N'10.0.12'); END;  COMMIT;
GO
BEGIN TRANSACTION; IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005131043_AddNumberOfContainers' ) BEGIN     ALTER TABLE [Operations] ADD [NumberOfContainers] int NOT NULL DEFAULT 0; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005131043_AddNumberOfContainers' ) BEGIN     DECLARE @var53 nvarchar(max);     SELECT @var53 = QUOTENAME([d].[name])     FROM [sys].[default_constraints] [d]     INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]     WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'ContainerType');     IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var53 + ';');     ALTER TABLE [OperationContainers] ALTER COLUMN [ContainerType] int NULL; END;  IF NOT EXISTS (     SELECT * FROM [__EFMigrationsHistory]     WHERE [MigrationId] = N'20261005131043_AddNumberOfContainers' ) BEGIN     INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])     VALUES (N'20261005131043_AddNumberOfContainers', N'10.0.12'); END;  COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    ALTER TABLE [Operations] ADD [Source] nvarchar(8) NOT NULL DEFAULT N'form';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE TABLE [audit_log] (
        [Id] bigint NOT NULL IDENTITY,
        [OperationId] uniqueidentifier NOT NULL,
        [BeforeStatus] nvarchar(20) NOT NULL,
        [AfterStatus] nvarchar(20) NOT NULL,
        [UserId] nvarchar(128) NOT NULL,
        [AtUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_audit_log] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_audit_log_StatusChanged] CHECK ([BeforeStatus] <> [AfterStatus]),
        CONSTRAINT [FK_audit_log_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE TABLE [FclSuggestions] (
        [Id] uniqueidentifier NOT NULL,
        [ImportOperationId] uniqueidentifier NOT NULL,
        [ImportContainerId] uniqueidentifier NOT NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [ClearanceType] nvarchar(20) NOT NULL,
        [BookingNumber] nvarchar(120) NOT NULL,
        [ShippingLineId] uniqueidentifier NOT NULL,
        [POLId] uniqueidentifier NOT NULL,
        [POWId] uniqueidentifier NULL,
        [CutOffDate] date NULL,
        [ContainerNumber] nvarchar(11) NOT NULL,
        [ContainerType] int NOT NULL,
        [Destination] nvarchar(200) NOT NULL,
        [RequiredLoadingDate] date NOT NULL,
        [RequiredLoadingTime] time NOT NULL,
        [FreeTimeMatch] bit NOT NULL,
        [ShippingLineMatch] bit NOT NULL,
        [PodPolMatch] bit NOT NULL,
        [OffloadingLoadingTimeMatch] bit NOT NULL,
        [ContainerSizeMatch] bit NOT NULL,
        [DestinationMatch] bit NOT NULL,
        [MatchCount] int NOT NULL,
        [Status] nvarchar(20) NOT NULL DEFAULT N'Pending',
        [CreatedAtUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_FclSuggestions] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_FclSuggestions_ClearanceType] CHECK ([ClearanceType] IN ('Bosla', 'Cert')),
        CONSTRAINT [CK_FclSuggestions_MatchCount] CHECK ([MatchCount] >= 5 AND [MatchCount] <= 6),
        CONSTRAINT [CK_FclSuggestions_Status] CHECK ([Status] IN ('Pending', 'Approved')),
        CONSTRAINT [FK_FclSuggestions_OperationContainers_ImportContainerId] FOREIGN KEY ([ImportContainerId]) REFERENCES [OperationContainers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FclSuggestions_Operations_ImportOperationId] FOREIGN KEY ([ImportOperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FclSuggestions_Ports_POLId] FOREIGN KEY ([POLId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FclSuggestions_Ports_POWId] FOREIGN KEY ([POWId]) REFERENCES [Ports] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FclSuggestions_ShippingLines_ShippingLineId] FOREIGN KEY ([ShippingLineId]) REFERENCES [ShippingLines] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE TABLE [FclApprovals] (
        [Id] uniqueidentifier NOT NULL,
        [SuggestionId] uniqueidentifier NOT NULL,
        [CreatedExportOperationId] uniqueidentifier NOT NULL,
        [ApprovalKey] nvarchar(300) NOT NULL,
        [ApprovedByUserId] nvarchar(128) NOT NULL,
        [ApprovedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_FclApprovals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FclApprovals_FclSuggestions_SuggestionId] FOREIGN KEY ([SuggestionId]) REFERENCES [FclSuggestions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FclApprovals_Operations_CreatedExportOperationId] FOREIGN KEY ([CreatedExportOperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    EXEC(N'ALTER TABLE [Operations] ADD CONSTRAINT [CK_Operations_Source] CHECK ([Source] IN (''form'', ''import'', ''fcl''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_audit_log_OperationId_AtUtc] ON [audit_log] ([OperationId], [AtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FclApprovals_CreatedExportOperationId] ON [FclApprovals] ([CreatedExportOperationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FclApprovals_SuggestionId] ON [FclApprovals] ([SuggestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE UNIQUE INDEX [UX_FclApprovals_ApprovalKey] ON [FclApprovals] ([ApprovalKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_ImportContainerId] ON [FclSuggestions] ([ImportContainerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_ImportOperationId] ON [FclSuggestions] ([ImportOperationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_POLId] ON [FclSuggestions] ([POLId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_POWId] ON [FclSuggestions] ([POWId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_ShippingLineId] ON [FclSuggestions] ([ShippingLineId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_Status] ON [FclSuggestions] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019062541_AddTptFclAndStatusAudit'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019062541_AddTptFclAndStatusAudit', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    ALTER TABLE [FclApprovals] DROP CONSTRAINT [FK_FclApprovals_Operations_CreatedExportOperationId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    DROP INDEX [IX_FclApprovals_CreatedExportOperationId] ON [FclApprovals];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    EXEC sp_rename N'[FclApprovals].[CreatedExportOperationId]', N'ExportOperationId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    ALTER TABLE [FclSuggestions] ADD [ExportOperationId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_ExportOperationId] ON [FclSuggestions] ([ExportOperationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    CREATE INDEX [IX_FclApprovals_ExportOperationId] ON [FclApprovals] ([ExportOperationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    ALTER TABLE [FclApprovals] ADD CONSTRAINT [FK_FclApprovals_Operations_ExportOperationId] FOREIGN KEY ([ExportOperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    ALTER TABLE [FclSuggestions] ADD CONSTRAINT [FK_FclSuggestions_Operations_ExportOperationId] FOREIGN KEY ([ExportOperationId]) REFERENCES [Operations] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019073430_LinkFclToExistingExportOperation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019073430_LinkFclToExistingExportOperation', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019075400_Allow200ContainerOrdinals'
)
BEGIN
    ALTER TABLE [OperationContainers] DROP CONSTRAINT [CK_OperationContainer_Ordinal];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019075400_Allow200ContainerOrdinals'
)
BEGIN
    EXEC(N'ALTER TABLE [OperationContainers] ADD CONSTRAINT [CK_OperationContainer_Ordinal] CHECK ([Ordinal] >= 1 AND [Ordinal] <= 200)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019075400_Allow200ContainerOrdinals'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019075400_Allow200ContainerOrdinals', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019081240_AddExportDestination'
)
BEGIN
    ALTER TABLE [ExportDetails] ADD [Destination] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019081240_AddExportDestination'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019081240_AddExportDestination', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019134647_MakeExportDestinationNullable'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'Destination');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [ExportDetails] ALTER COLUMN [Destination] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019134647_MakeExportDestinationNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019134647_MakeExportDestinationNullable', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019174543_AddContainerDestinationAndFreeTime'
)
BEGIN
    ALTER TABLE [OperationContainers] ADD [Destination] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019174543_AddContainerDestinationAndFreeTime'
)
BEGIN
    ALTER TABLE [OperationContainers] ADD [FreeTimeTill] date NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019174543_AddContainerDestinationAndFreeTime'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019174543_AddContainerDestinationAndFreeTime', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019182359_AddExportContainerToFclSuggestions'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FclSuggestions]') AND [c].[name] = N'Destination');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [FclSuggestions] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [FclSuggestions] ALTER COLUMN [Destination] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019182359_AddExportContainerToFclSuggestions'
)
BEGIN
    ALTER TABLE [FclSuggestions] ADD [ExportContainerId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019182359_AddExportContainerToFclSuggestions'
)
BEGIN
    CREATE INDEX [IX_FclSuggestions_ExportContainerId] ON [FclSuggestions] ([ExportContainerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019182359_AddExportContainerToFclSuggestions'
)
BEGIN
    ALTER TABLE [FclSuggestions] ADD CONSTRAINT [FK_FclSuggestions_OperationContainers_ExportContainerId] FOREIGN KEY ([ExportContainerId]) REFERENCES [OperationContainers] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019182359_AddExportContainerToFclSuggestions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019182359_AddExportContainerToFclSuggestions', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019193341_RemoveDuplicateContainerFields'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'Destination');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [Destination];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019193341_RemoveDuplicateContainerFields'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'FreeTimeTill');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [FreeTimeTill];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019193341_RemoveDuplicateContainerFields'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'Destination');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [Destination];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019193341_RemoveDuplicateContainerFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019193341_RemoveDuplicateContainerFields', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020063100_AddOutdatedFclSuggestions'
)
BEGIN
    ALTER TABLE [FclSuggestions] DROP CONSTRAINT [CK_FclSuggestions_Status];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020063100_AddOutdatedFclSuggestions'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_FclSuggestions_ImportContainerId_ExportContainerId] ON [FclSuggestions] ([ImportContainerId], [ExportContainerId]) WHERE [ExportContainerId] IS NOT NULL AND [Status] = ''Pending''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020063100_AddOutdatedFclSuggestions'
)
BEGIN
    EXEC(N'ALTER TABLE [FclSuggestions] ADD CONSTRAINT [CK_FclSuggestions_Status] CHECK ([Status] IN (''Pending'', ''Approved'', ''Outdated''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020063100_AddOutdatedFclSuggestions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020063100_AddOutdatedFclSuggestions', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020072038_RremoveOutdatedFclSuggestions'
)
BEGIN
    ALTER TABLE [FclSuggestions] DROP CONSTRAINT [CK_FclSuggestions_Status];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020072038_RremoveOutdatedFclSuggestions'
)
BEGIN
    EXEC(N'ALTER TABLE [FclSuggestions] ADD CONSTRAINT [CK_FclSuggestions_Status] CHECK ([Status] IN (''Pending'', ''Approved''))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020072038_RremoveOutdatedFclSuggestions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020072038_RremoveOutdatedFclSuggestions', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020105545_NormalizeImportExportDuplicateKeys'
)
BEGIN
    ALTER TABLE [Operations] ADD [DuplicateCheckKey] nvarchar(400) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020105545_NormalizeImportExportDuplicateKeys'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Operations_OperationType_DuplicateCheckKey] ON [Operations] ([OperationType], [DuplicateCheckKey]) WHERE [IsDeleted] = 0 AND [DuplicateCheckKey] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020105545_NormalizeImportExportDuplicateKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020105545_NormalizeImportExportDuplicateKeys', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DROP INDEX [UX_Operations_OperationType_DuplicateCheckKey] ON [Operations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'DeletedAtUtc');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Operations] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'DeletedByName');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [Operations] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'DeletedByUserId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [Operations] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'IsDeleted');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [Operations] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [Operations] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'UpdatedByName');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [Operations] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Operations]') AND [c].[name] = N'UpdatedByUserId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Operations] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [Operations] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'CreatedAtUtc');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'CreatedByName');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'CreatedByUserId');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'DeletedAtUtc');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var16 nvarchar(max);
    SELECT @var16 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'DeletedByName');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var16 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var17 nvarchar(max);
    SELECT @var17 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'DeletedByUserId');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var17 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var18 nvarchar(max);
    SELECT @var18 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'IsDeleted');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var18 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var19 nvarchar(max);
    SELECT @var19 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var19 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var20 nvarchar(max);
    SELECT @var20 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'UpdatedByName');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var20 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var21 nvarchar(max);
    SELECT @var21 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OperationContainers]') AND [c].[name] = N'UpdatedByUserId');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [OperationContainers] DROP CONSTRAINT ' + @var21 + ';');
    ALTER TABLE [OperationContainers] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var22 nvarchar(max);
    SELECT @var22 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'CreatedAtUtc');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var22 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var23 nvarchar(max);
    SELECT @var23 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'CreatedByName');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var23 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var24 nvarchar(max);
    SELECT @var24 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'CreatedByUserId');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var24 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var25 nvarchar(max);
    SELECT @var25 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'DeletedAtUtc');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var25 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var26 nvarchar(max);
    SELECT @var26 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'DeletedByName');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var26 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var27 nvarchar(max);
    SELECT @var27 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'DeletedByUserId');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var27 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var28 nvarchar(max);
    SELECT @var28 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'IsDeleted');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var28 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var29 nvarchar(max);
    SELECT @var29 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var29 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var30 nvarchar(max);
    SELECT @var30 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'UpdatedByName');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var30 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var31 nvarchar(max);
    SELECT @var31 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportDetails]') AND [c].[name] = N'UpdatedByUserId');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [ImportDetails] DROP CONSTRAINT ' + @var31 + ';');
    ALTER TABLE [ImportDetails] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var32 nvarchar(max);
    SELECT @var32 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'CreatedAtUtc');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var32 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [CreatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var33 nvarchar(max);
    SELECT @var33 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'CreatedByName');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var33 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [CreatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var34 nvarchar(max);
    SELECT @var34 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'CreatedByUserId');
    IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var34 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [CreatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var35 nvarchar(max);
    SELECT @var35 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'DeletedAtUtc');
    IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var35 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [DeletedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var36 nvarchar(max);
    SELECT @var36 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'DeletedByName');
    IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var36 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [DeletedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var37 nvarchar(max);
    SELECT @var37 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'DeletedByUserId');
    IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var37 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [DeletedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var38 nvarchar(max);
    SELECT @var38 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'IsDeleted');
    IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var38 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var39 nvarchar(max);
    SELECT @var39 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'UpdatedAtUtc');
    IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var39 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [UpdatedAtUtc];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var40 nvarchar(max);
    SELECT @var40 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'UpdatedByName');
    IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var40 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [UpdatedByName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    DECLARE @var41 nvarchar(max);
    SELECT @var41 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportDetails]') AND [c].[name] = N'UpdatedByUserId');
    IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [ExportDetails] DROP CONSTRAINT ' + @var41 + ';');
    ALTER TABLE [ExportDetails] DROP COLUMN [UpdatedByUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Operations_OperationType_DuplicateCheckKey] ON [Operations] ([OperationType], [DuplicateCheckKey]) WHERE [DuplicateCheckKey] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020170257_AddOperationCreationAuditAndRemoveAuditableAndSoftable', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021173658_UpdateFclSuggestionMatchCountConstraint'
)
BEGIN
    ALTER TABLE [FclSuggestions] DROP CONSTRAINT [CK_FclSuggestions_MatchCount];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021173658_UpdateFclSuggestionMatchCountConstraint'
)
BEGIN
    EXEC(N'ALTER TABLE [FclSuggestions] ADD CONSTRAINT [CK_FclSuggestions_MatchCount] CHECK ([MatchCount] = 6)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021173658_UpdateFclSuggestionMatchCountConstraint'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021173658_UpdateFclSuggestionMatchCountConstraint', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    ALTER TABLE [FclApprovals] ADD [ImportContainerId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN

                    UPDATE a
                    SET a.ImportContainerId = s.ImportContainerId
                    FROM FclApprovals a
                    INNER JOIN FclSuggestions s ON a.SuggestionId = s.Id
                    WHERE a.ImportContainerId IS NULL;
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN

                    DELETE FROM FclApprovals
                    WHERE ImportContainerId IS NULL;
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    DECLARE @var42 nvarchar(max);
    SELECT @var42 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FclApprovals]') AND [c].[name] = N'ImportContainerId');
    IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [FclApprovals] DROP CONSTRAINT ' + @var42 + ';');
    ALTER TABLE [FclApprovals] ALTER COLUMN [ImportContainerId] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    CREATE UNIQUE INDEX [UX_FclApprovals_ImportContainerId] ON [FclApprovals] ([ImportContainerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    ALTER TABLE [FclApprovals] ADD CONSTRAINT [FK_FclApprovals_OperationContainers_ImportContainerId] FOREIGN KEY ([ImportContainerId]) REFERENCES [OperationContainers] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    ALTER TABLE [FclApprovals] DROP CONSTRAINT [FK_FclApprovals_FclSuggestions_SuggestionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    DROP INDEX [IX_FclApprovals_SuggestionId] ON [FclApprovals];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    DECLARE @var43 nvarchar(max);
    SELECT @var43 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FclApprovals]') AND [c].[name] = N'SuggestionId');
    IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [FclApprovals] DROP CONSTRAINT ' + @var43 + ';');
    ALTER TABLE [FclApprovals] DROP COLUMN [SuggestionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    DECLARE @var44 nvarchar(max);
    SELECT @var44 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FclApprovals]') AND [c].[name] = N'ExportOperationId');
    IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [FclApprovals] DROP CONSTRAINT ' + @var44 + ';');
    ALTER TABLE [FclApprovals] ALTER COLUMN [ExportOperationId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    DROP TABLE [FclSuggestions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027095500_UpdateFclApprovalsDirectImportContainer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261027095500_UpdateFclApprovalsDirectImportContainer', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027133000_AddDestinationTable'
)
BEGIN
    CREATE TABLE [Destinations] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Destinations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027133000_AddDestinationTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Destinations_Name] ON [Destinations] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027133000_AddDestinationTable'
)
BEGIN

                    IF NOT EXISTS (SELECT 1 FROM Destinations WHERE Name = '10TH OF RAMADAN')
                    BEGIN
                        INSERT INTO Destinations (Id, Name, IsActive) VALUES
                        (NEWID(), '10TH OF RAMADAN', 1),
                        (NEWID(), '6TH OF OCTOBER', 1),
                        (NEWID(), 'ABU RAWASH', 1),
                        (NEWID(), 'ALEXANDRIA', 1),
                        (NEWID(), 'BORG EL ARAB', 1),
                        (NEWID(), 'EL SADAT', 1),
                        (NEWID(), 'OCTOBER', 1),
                        (NEWID(), 'SOKHNA', 1),
                        (NEWID(), 'WADI EL NATRUN', 1);
                    END
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027133000_AddDestinationTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261027133000_AddDestinationTable', N'10.0.12');
END;

COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103104500_AddAbbreviationToShippingLines'
)
BEGIN
    ALTER TABLE [ShippingLines] ADD [Abbreviation] nvarchar(50) NULL;

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103104500_AddAbbreviationToShippingLines', N'10.0.12');
END;
COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103101518_TestCheck'
)
BEGIN
    IF COL_LENGTH(N'[Vehicles]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [Vehicles] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[Vehicles]', N'DeletedByName') IS NULL
        ALTER TABLE [Vehicles] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Vehicles]', N'DeletedByUserId') IS NULL
        ALTER TABLE [Vehicles] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Vehicles]', N'IsDeleted') IS NULL
        ALTER TABLE [Vehicles] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF COL_LENGTH(N'[Transporters]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [Transporters] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[Transporters]', N'DeletedByName') IS NULL
        ALTER TABLE [Transporters] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Transporters]', N'DeletedByUserId') IS NULL
        ALTER TABLE [Transporters] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Transporters]', N'IsDeleted') IS NULL
        ALTER TABLE [Transporters] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF COL_LENGTH(N'[ShippingLines]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [ShippingLines] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[ShippingLines]', N'DeletedByName') IS NULL
        ALTER TABLE [ShippingLines] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[ShippingLines]', N'DeletedByUserId') IS NULL
        ALTER TABLE [ShippingLines] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[ShippingLines]', N'IsDeleted') IS NULL
        ALTER TABLE [ShippingLines] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF COL_LENGTH(N'[Ports]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [Ports] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[Ports]', N'DeletedByName') IS NULL
        ALTER TABLE [Ports] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Ports]', N'DeletedByUserId') IS NULL
        ALTER TABLE [Ports] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Ports]', N'IsDeleted') IS NULL
        ALTER TABLE [Ports] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF COL_LENGTH(N'[Operations]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [Operations] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[Operations]', N'DeletedByName') IS NULL
        ALTER TABLE [Operations] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Operations]', N'DeletedByUserId') IS NULL
        ALTER TABLE [Operations] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Operations]', N'IsDeleted') IS NULL
        ALTER TABLE [Operations] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF COL_LENGTH(N'[ExportDetails]', N'PODId') IS NULL
        ALTER TABLE [ExportDetails] ADD [PODId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Drivers]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [Drivers] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[Drivers]', N'DeletedByName') IS NULL
        ALTER TABLE [Drivers] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Drivers]', N'DeletedByUserId') IS NULL
        ALTER TABLE [Drivers] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Drivers]', N'IsDeleted') IS NULL
        ALTER TABLE [Drivers] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF COL_LENGTH(N'[Destinations]', N'DeletedAtUtc') IS NULL
        ALTER TABLE [Destinations] ADD [DeletedAtUtc] datetime2 NULL;

    IF COL_LENGTH(N'[Destinations]', N'DeletedByName') IS NULL
        ALTER TABLE [Destinations] ADD [DeletedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Destinations]', N'DeletedByUserId') IS NULL
        ALTER TABLE [Destinations] ADD [DeletedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Destinations]', N'IsDeleted') IS NULL
        ALTER TABLE [Destinations] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ExportDetails_PODId' AND object_id = OBJECT_ID(N'[ExportDetails]'))
        CREATE INDEX [IX_ExportDetails_PODId] ON [ExportDetails] ([PODId]);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ExportDetails_Ports_PODId' AND parent_object_id = OBJECT_ID(N'[ExportDetails]'))
        ALTER TABLE [ExportDetails] ADD CONSTRAINT [FK_ExportDetails_Ports_PODId] FOREIGN KEY ([PODId]) REFERENCES [Ports] ([Id]);

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103101518_TestCheck', N'10.0.12');
END;
COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105103000_AddOperationsPerformanceIndex'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operations_Type_Deleted_Status_Created' AND object_id = OBJECT_ID(N'[Operations]'))
        CREATE INDEX [IX_Operations_Type_Deleted_Status_Created] ON [Operations] ([OperationType], [IsDeleted], [Status], [CreatedAtUtc]);

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261105103000_AddOperationsPerformanceIndex', N'10.0.12');
END;
COMMIT;
GO
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105150000_AddOperationSoftLockFields'
)
BEGIN
    IF COL_LENGTH(N'[Operations]', N'LockedByUserId') IS NULL
        ALTER TABLE [Operations] ADD [LockedByUserId] uniqueidentifier NULL;

    IF COL_LENGTH(N'[Operations]', N'LockedByName') IS NULL
        ALTER TABLE [Operations] ADD [LockedByName] nvarchar(200) NULL;

    IF COL_LENGTH(N'[Operations]', N'LockedAtUtc') IS NULL
        ALTER TABLE [Operations] ADD [LockedAtUtc] datetime2 NULL;

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261105150000_AddOperationSoftLockFields', N'10.0.12');
END;
COMMIT;
GO
