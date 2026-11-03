SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
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

/* ===========================================================================
   SEED DATA SECTION (Idempotent: Safe to re-run without duplicate conflicts)
   =========================================================================== */

-- Seed Customers
PRINT 'Seeding Customers...';
MERGE INTO [Customers] AS target
USING (VALUES
    (N'MIDEA ELECTRICS EGYPT', N'Representative - Midea Electrics Egypt', N'01020000001', N'ops.midea.electrics.egypt@customer.ils.com'),
    (N'MISR REFRIGERATION AND AIR CONDITIONING MFG CO . S.A.E', N'Representative - Misr Refrigeration And Air Conditioning Mfg Co . S.A.E', N'01020000002', N'ops.misr.refrigeration.and.air.conditio@customer.ils.com'),
    (N'DICE FOR READYMADE GARMENTS', N'Representative - Dice For Readymade Garments', N'01020000003', N'ops.dice.for.readymade.garments@customer.ils.com'),
    (N'MIDEA (EGYPT) KITCHEN AND WATER HEATER APPLIANCE CO. L.T.D', N'Representative - Midea (Egypt) Kitchen And Water Heater Appliance Co. L.T.D', N'01020000004', N'ops.midea.egypt.kitchen.and.water.heate@customer.ils.com'),
    (N'GERMAN CO. FOR MANUFACTURING BATTERIES', N'Representative - German Co. For Manufacturing Batteries', N'01020000005', N'ops.german.co.for.manufacturing.batteri@customer.ils.com'),
    (N'HAIER ELECTRIC EGYPT LIMITED L.L.C', N'Representative - Haier Electric Egypt Limited L.L.C', N'01020000006', N'ops.haier.electric.egypt.limited.l.l.c@customer.ils.com'),
    (N'WORLDWIDE LOGISTICS SINGAPORE PTE. LTD.', N'Representative - Worldwide Logistics Singapore Pte. Ltd.', N'01020000007', N'ops.worldwide.logistics.singapore.pte.l@customer.ils.com'),
    (N'CMBI', N'Representative - Cmbi', N'01020000008', N'ops.cmbi@customer.ils.com'),
    (N'ZEIN AL ABDEEN FOR PLASTIC INDUSTRIES', N'Representative - Zein Al Abdeen For Plastic Industries', N'01020000009', N'ops.zein.al.abdeen.for.plastic.industri@customer.ils.com'),
    (N'RAYA ELECTRIC FOR MANUFACTURING', N'Representative - Raya Electric For Manufacturing', N'01020000010', N'ops.raya.electric.for.manufacturing@customer.ils.com'),
    (N'EGYPT CHUAN JIN NUO CHEMICAL CO LTD', N'Representative - Egypt Chuan Jin Nuo Chemical Co Ltd', N'01020000011', N'ops.egypt.chuan.jin.nuo.chemical.co.ltd@customer.ils.com'),
    (N'HENGTONG OPTIC ELECTRIC EGYPT CO. S.A.E', N'Representative - Hengtong Optic Electric Egypt Co. S.A.E', N'01020000012', N'ops.hengtong.optic.electric.egypt.co.s@customer.ils.com'),
    (N'NATIONAL DRILLING COMPANY (NDC)', N'Representative - National Drilling Company (Ndc)', N'01020000013', N'ops.national.drilling.company.ndc@customer.ils.com'),
    (N'AL ALAMIA FACTORY FOR PLASTIC INDUSTRIES', N'Representative - Al Alamia Factory For Plastic Industries', N'01020000014', N'ops.al.alamia.factory.for.plastic.indus@customer.ils.com'),
    (N'MTM INTERNATIONAL IMPORT & EXPORT.', N'Representative - Mtm International Import & Export.', N'01020000015', N'ops.mtm.international.import.export@customer.ils.com'),
    (N'LONDON FOODS COMPANY', N'Representative - London Foods Company', N'01020000016', N'ops.london.foods.company@customer.ils.com'),
    (N'CSCEC STEEL EGYPT', N'Representative - Cscec Steel Egypt', N'01020000017', N'ops.cscec.steel.egypt@customer.ils.com'),
    (N'ROYAL EL-MINYA CEMENT', N'Representative - Royal El-Minya Cement', N'01020000018', N'ops.royal.el.minya.cement@customer.ils.com'),
    (N'EGYPTIAN CO. FOR INTERNATIONAL TOURISTIC PROJECTS', N'Representative - Egyptian Co. For International Touristic Projects', N'01020000019', N'ops.egyptian.co.for.international.touri@customer.ils.com'),
    (N'ORIENT INTERNATIONAL TRADING LANDMARK G.', N'Representative - Orient International Trading Landmark G.', N'01020000020', N'ops.orient.international.trading.landma@customer.ils.com'),
    (N'FREE AIR COMPANY FOR INDUSTRY AND TRADING CO', N'Representative - Free Air Company For Industry And Trading Co', N'01020000021', N'ops.free.air.company.for.industry.and.t@customer.ils.com'),
    (N'SUNREV SOLAR LLC', N'Representative - Sunrev Solar Llc', N'01020000022', N'ops.sunrev.solar.llc@customer.ils.com'),
    (N'EL-MOHANDES CO.FOR TARDING AND MANUFACTURING CHEMICALS', N'Representative - El-Mohandes Co.For Tarding And Manufacturing Chemicals', N'01020000023', N'ops.el.mohandes.co.for.tarding.and.manu@customer.ils.com'),
    (N'DECOMIX FOR DEVELOPING AND PRODUCING', N'Representative - Decomix For Developing And Producing', N'01020000024', N'ops.decomix.for.developing.and.producin@customer.ils.com'),
    (N'TANK FOR TRADE AND INDUSTRY', N'Representative - Tank For Trade And Industry', N'01020000025', N'ops.tank.for.trade.and.industry@customer.ils.com'),
    (N'EL MARAKBY STEEL', N'Representative - El Marakby Steel', N'01020000026', N'ops.el.marakby.steel@customer.ils.com'),
    (N'XINXING DUCTILE IRON PIPES INDUSTRY CO., S.A.E.', N'Representative - Xinxing Ductile Iron Pipes Industry Co., S.A.E.', N'01020000027', N'ops.xinxing.ductile.iron.pipes.industry@customer.ils.com'),
    (N'FUTURE PIPE INDUSTRIES SAE', N'Representative - Future Pipe Industries Sae', N'01020000028', N'ops.future.pipe.industries.sae@customer.ils.com'),
    (N'HIGA INTELLIGENCE MANUFACTURING “EGYPT” CO.,LTD', N'Representative - Higa Intelligence Manufacturing “Egypt” Co.,Ltd', N'01020000029', N'ops.higa.intelligence.manufacturing.egy@customer.ils.com'),
    (N'GRAND FREIGHT SERVICE LIMITED', N'Representative - Grand Freight Service Limited', N'01020000030', N'ops.grand.freight.service.limited@customer.ils.com'),
    (N'SC ZONE UTILITIES', N'Representative - Sc Zone Utilities', N'01020000031', N'ops.sc.zone.utilities@customer.ils.com'),
    (N'XD EGEMAC', N'Representative - Xd Egemac', N'01020000032', N'ops.xd.egemac@customer.ils.com'),
    (N'MANA FIBRO LTD', N'Representative - Mana Fibro Ltd', N'01020000033', N'ops.mana.fibro.ltd@customer.ils.com'),
    (N'ISS-PALUMBO EGYPT', N'Representative - Iss-Palumbo Egypt', N'01020000034', N'ops.iss.palumbo.egypt@customer.ils.com'),
    (N'AL SAMY FOOD COMPANY', N'Representative - Al Samy Food Company', N'01020000035', N'ops.al.samy.food.company@customer.ils.com'),
    (N'THE WORLD COMPANY FOR DRAWING AND MANUFACTURING WIRES', N'Representative - The World Company For Drawing And Manufacturing Wires', N'01020000036', N'ops.the.world.company.for.drawing.and.m@customer.ils.com'),
    (N'ELITE SOLAR GREEN ENERGY COMPANY LIMITED', N'Representative - Elite Solar Green Energy Company Limited', N'01020000037', N'ops.elite.solar.green.energy.company.li@customer.ils.com'),
    (N'SHENZHEN WONDER SUPPLY CHAIN CO., LTD', N'Representative - Shenzhen Wonder Supply Chain Co., Ltd', N'01020000038', N'ops.shenzhen.wonder.supply.chain.co.ltd@customer.ils.com'),
    (N'EGYPTIAN INTERNATIONAL CO. FOR FOOD INDUSTRIES', N'Representative - Egyptian International Co. For Food Industries', N'01020000039', N'ops.egyptian.international.co.for.food@customer.ils.com'),
    (N'EGYPT CADY TEXTILE CO, S.A.E', N'Representative - Egypt Cady Textile Co, S.A.E', N'01020000040', N'ops.egypt.cady.textile.co.s.a.e@customer.ils.com'),
    (N'LEONI WIRING SYSTEMS EGYPT SAE', N'Representative - Leoni Wiring Systems Egypt Sae', N'01020000041', N'ops.leoni.wiring.systems.egypt.sae@customer.ils.com'),
    (N'MIRACO DEVELOPMENT SERVICES & TRADING CO S.A.E.', N'Representative - Miraco Development Services & Trading Co S.A.E.', N'01020000042', N'ops.miraco.development.services.trading@customer.ils.com'),
    (N'MEDITEX', N'Representative - Meditex', N'01020000043', N'ops.meditex@customer.ils.com'),
    (N'CTP', N'Representative - Ctp', N'01020000044', N'ops.ctp@customer.ils.com'),
    (N'AL-SHARK EL -ARABI FOR IMP AND EXP CO.,', N'Representative - Al-Shark El -Arabi For Imp And Exp Co.,', N'01020000045', N'ops.al.shark.el.arabi.for.imp.and.exp.c@customer.ils.com'),
    (N'GRAND FREIGHT SERVICE LIMITED.', N'Representative - Grand Freight Service Limited.', N'01020000046', N'ops.grand.freight.service.limited.46@customer.ils.com'),
    (N'THREE BROTHERS COMPANY.', N'Representative - Three Brothers Company.', N'01020000047', N'ops.three.brothers.company@customer.ils.com'),
    (N'GOLDEN SUN INDUSTRY CO', N'Representative - Golden Sun Industry Co', N'01020000048', N'ops.golden.sun.industry.co@customer.ils.com'),
    (N'EGYPTIAN RUBBER AND PLASTIC INDUSTRIES (ERPI )', N'Representative - Egyptian Rubber And Plastic Industries (Erpi )', N'01020000049', N'ops.egyptian.rubber.and.plastic.industr@customer.ils.com'),
    (N'MASHARIA ELTAKA FOR BATTERIES', N'Representative - Masharia Eltaka For Batteries', N'01020000050', N'ops.masharia.eltaka.for.batteries@customer.ils.com'),
    (N'ARROW FOR MEDICAL NECESSITIES', N'Representative - Arrow For Medical Necessities', N'01020000051', N'ops.arrow.for.medical.necessities@customer.ils.com'),
    (N'MISR REFRIGERATION & AIR CONDITIONING MFG. CO.', N'Representative - Misr Refrigeration & Air Conditioning Mfg. Co.', N'01020000052', N'ops.misr.refrigeration.air.conditioning@customer.ils.com'),
    (N'AMS ADVANCED MEDICAL SOLUTION', N'Representative - Ams Advanced Medical Solution', N'01020000053', N'ops.ams.advanced.medical.solution@customer.ils.com'),
    (N'ASFOUR FOR MINING AND REFRACTORIES & IMPORT & EXPORT', N'Representative - Asfour For Mining And Refractories & Import & Export', N'01020000054', N'ops.asfour.for.mining.and.refractories@customer.ils.com'),
    (N'UNITED CO FOR FOUNDRIES', N'Representative - United Co For Foundries', N'01020000055', N'ops.united.co.for.foundries@customer.ils.com'),
    (N'AL DAHB IMPORT & EXPORT', N'Representative - Al Dahb Import & Export', N'01020000056', N'ops.al.dahb.import.export@customer.ils.com'),
    (N'AJE GROUP EGYPT FOR SOFT DRINKS S.A.E.', N'Representative - Aje Group Egypt For Soft Drinks S.A.E.', N'01020000057', N'ops.aje.group.egypt.for.soft.drinks.s.a@customer.ils.com'),
    (N'EXPRESS SRL', N'Representative - Express Srl', N'01020000058', N'ops.express.srl@customer.ils.com'),
    (N'MASS FOR MEDICAL NECESSITIES', N'Representative - Mass For Medical Necessities', N'01020000059', N'ops.mass.for.medical.necessities@customer.ils.com'),
    (N'EGYPTIAN CEMENT T FOR CEMENT PROJECTS MANAGEMENT S.A.E', N'Representative - Egyptian Cement T For Cement Projects Management S.A.E', N'01020000060', N'ops.egyptian.cement.t.for.cement.projec@customer.ils.com'),
    (N'LTS GLOBAL SOLUTIONS', N'Representative - Lts Global Solutions', N'01020000061', N'ops.lts.global.solutions@customer.ils.com'),
    (N'EGYPTIAN GRINDING ADVANCED TECHNOLOGY ''EGAT'' S.A.,', N'Representative - Egyptian Grinding Advanced Technology ''Egat'' S.A.,', N'01020000062', N'ops.egyptian.grinding.advanced.technolo@customer.ils.com'),
    (N'NOVO PACK FOR PACKAGING', N'Representative - Novo Pack For Packaging', N'01020000063', N'ops.novo.pack.for.packaging@customer.ils.com'),
    (N'WORLDWIDE LOGISTICS CO., LTD.', N'Representative - Worldwide Logistics Co., Ltd.', N'01020000064', N'ops.worldwide.logistics.co.ltd@customer.ils.com'),
    (N'ORASCOM TRADING CO. S.A.E (OA)', N'Representative - Orascom Trading Co. S.A.E (Oa)', N'01020000065', N'ops.orascom.trading.co.s.a.e.oa@customer.ils.com'),
    (N'SILVER EDGE TECHNOLOGY', N'Representative - Silver Edge Technology', N'01020000066', N'ops.silver.edge.technology@customer.ils.com'),
    (N'ENKANA PRINTING INKS', N'Representative - Enkana Printing Inks', N'01020000067', N'ops.enkana.printing.inks@customer.ils.com'),
    (N'HAO BAO ELECTRONIC TECHNOLOGY . CO', N'Representative - Hao Bao Electronic Technology . Co', N'01020000068', N'ops.hao.bao.electronic.technology.co@customer.ils.com'),
    (N'CLEOPATRA INDUSTRIES FOR BUILDING MATERIALS', N'Representative - Cleopatra Industries For Building Materials', N'01020000069', N'ops.cleopatra.industries.for.building.m@customer.ils.com'),
    (N'MARMOX EGYPT', N'Representative - Marmox Egypt', N'01020000070', N'ops.marmox.egypt@customer.ils.com'),
    (N'GOKIN SOLAR HONGKONG COMPANY', N'Representative - Gokin Solar Hongkong Company', N'01020000071', N'ops.gokin.solar.hongkong.company@customer.ils.com'),
    (N'LINK LOGISTICS', N'Representative - Link Logistics', N'01020000072', N'ops.link.logistics@customer.ils.com'),
    (N'AUTO EGYPT FOR TRADING', N'Representative - Auto Egypt For Trading', N'01020000073', N'ops.auto.egypt.for.trading@customer.ils.com'),
    (N'ALROWAD ELDAWLIA COMPANY “PICO”', N'Representative - Alrowad Eldawlia Company “Pico”', N'01020000074', N'ops.alrowad.eldawlia.company.pico@customer.ils.com'),
    (N'JETONE GLOBAL FREIGHT CO .,LTD', N'Representative - Jetone Global Freight Co .,Ltd', N'01020000075', N'ops.jetone.global.freight.co.ltd@customer.ils.com'),
    (N'TIBA FOR TRADE & DISTRIBUTION', N'Representative - Tiba For Trade & Distribution', N'01020000076', N'ops.tiba.for.trade.distribution@customer.ils.com'),
    (N'AL BARAKA PAPER CONVERTING COMPANY', N'Representative - Al Baraka Paper Converting Company', N'01020000077', N'ops.al.baraka.paper.converting.company@customer.ils.com'),
    (N'CSG CARGO SYSTEMS GERMANY GMBH', N'Representative - Csg Cargo Systems Germany Gmbh', N'01020000078', N'ops.csg.cargo.systems.germany.gmbh@customer.ils.com'),
    (N'NOVOCHEM', N'Representative - Novochem', N'01020000079', N'ops.novochem@customer.ils.com'),
    (N'NCFI FOOD INDUSTRIES CO', N'Representative - Ncfi Food Industries Co', N'01020000080', N'ops.ncfi.food.industries.co@customer.ils.com'),
    (N'SHELL EGYPT TRADING', N'Representative - Shell Egypt Trading', N'01020000081', N'ops.shell.egypt.trading@customer.ils.com'),
    (N'ALEXANDRIA FERTILIZERS CO', N'Representative - Alexandria Fertilizers Co', N'01020000082', N'ops.alexandria.fertilizers.co@customer.ils.com'),
    (N'RAYA FOOD AND BEVERAGE DISTRIBUTION', N'Representative - Raya Food And Beverage Distribution', N'01020000083', N'ops.raya.food.and.beverage.distribution@customer.ils.com'),
    (N'NAVIGATORS SHIIPPING & LOGISTICA FZE', N'Representative - Navigators Shiipping & Logistica Fze', N'01020000084', N'ops.navigators.shiipping.logistica.fze@customer.ils.com'),
    (N'CHEMICALS FOR MODERN BUILDINGS', N'Representative - Chemicals For Modern Buildings', N'01020000085', N'ops.chemicals.for.modern.buildings@customer.ils.com'),
    (N'TRANSHAUL LOGISTICS LTD', N'Representative - Transhaul Logistics Ltd', N'01020000086', N'ops.transhaul.logistics.ltd@customer.ils.com'),
    (N'EL DAWLIA FOR SPARE PARTS', N'Representative - El Dawlia For Spare Parts', N'01020000087', N'ops.el.dawlia.for.spare.parts@customer.ils.com'),
    (N'ORIENT INTERNATIONAL TRADING', N'Representative - Orient International Trading', N'01020000088', N'ops.orient.international.trading@customer.ils.com'),
    (N'TECNO-SPUMA S.L.', N'Representative - Tecno-Spuma S.L.', N'01020000089', N'ops.tecno.spuma.s.l@customer.ils.com'),
    (N'ALBARIK INTERNATIONAL FOR IMPORT & EXPORT', N'Representative - Albarik International For Import & Export', N'01020000090', N'ops.albarik.international.for.import.ex@customer.ils.com'),
    (N'AL HARAM FOR IMPORT & EXPORT', N'Representative - Al Haram For Import & Export', N'01020000091', N'ops.al.haram.for.import.export@customer.ils.com'),
    (N'C & I LOGISTICS CO LTD', N'Representative - C & I Logistics Co Ltd', N'01020000092', N'ops.c.i.logistics.co.ltd@customer.ils.com'),
    (N'GERMAN ELEVATORS COMPANY', N'Representative - German Elevators Company', N'01020000093', N'ops.german.elevators.company@customer.ils.com'),
    (N'EL-SEWEDY ELECTRIC INDUSTRIES', N'Representative - El-Sewedy Electric Industries', N'01020000094', N'ops.el.sewedy.electric.industries@customer.ils.com'),
    (N'ROYAL MEDICAL INTERNATIONAL COMPANY', N'Representative - Royal Medical International Company', N'01020000095', N'ops.royal.medical.international.company@customer.ils.com'),
    (N'AMSEO INDUSTRIES', N'Representative - Amseo Industries', N'01020000096', N'ops.amseo.industries@customer.ils.com'),
    (N'ELARABY UNITED COMPANY FOR INDUSTRIAL AND COMMERCIAL INVESTMENT', N'Representative - Elaraby United Company For Industrial And Commercial Investment', N'01020000097', N'ops.elaraby.united.company.for.industri@customer.ils.com'),
    (N'MULTI SERVICE COMPANY WAVES', N'Representative - Multi Service Company Waves', N'01020000098', N'ops.multi.service.company.waves@customer.ils.com'),
    (N'CALDERYS FRANCE SA', N'Representative - Calderys France Sa', N'01020000099', N'ops.calderys.france.sa@customer.ils.com'),
    (N'HAIER SINGAPORE INVESTMENT HOLDING PTE. LTD.', N'Representative - Haier Singapore Investment Holding Pte. Ltd.', N'01020000100', N'ops.haier.singapore.investment.holding@customer.ils.com'),
    (N'PEARL SHIPPING AND LOGISTICS “ PSAL“', N'Representative - Pearl Shipping And Logistics “ Psal“', N'01020000101', N'ops.pearl.shipping.and.logistics.psal@customer.ils.com'),
    (N'OGTECH', N'Representative - Ogtech', N'01020000102', N'ops.ogtech@customer.ils.com'),
    (N'EL MARWAN FOR IMPORT', N'Representative - El Marwan For Import', N'01020000103', N'ops.el.marwan.for.import@customer.ils.com'),
    (N'MIRACO INTERNATIONAL TRADING', N'Representative - Miraco International Trading', N'01020000104', N'ops.miraco.international.trading@customer.ils.com'),
    (N'EGYPT FOR ENGINEERING INDUSTRIES', N'Representative - Egypt For Engineering Industries', N'01020000105', N'ops.egypt.for.engineering.industries@customer.ils.com'),
    (N'RED LINE EGYPT', N'Representative - Red Line Egypt', N'01020000106', N'ops.red.line.egypt@customer.ils.com'),
    (N'LIMITTRANS LOJISTIK VE TASIMACILIK LTD STI ( CLIENT )', N'Representative - Limittrans Lojistik Ve Tasimacilik Ltd Sti ( Client )', N'01020000107', N'ops.limittrans.lojistik.ve.tasimacilik@customer.ils.com'),
    (N'ILS EGYPT LTD', N'Representative - Ils Egypt Ltd', N'01020000108', N'ops.ils.egypt.ltd@customer.ils.com'),
    (N'CHONGQING XIANGYUN INTERNATIONAL TRANSPORTATION CO.,LTD', N'Representative - Chongqing Xiangyun International Transportation Co.,Ltd', N'01020000109', N'ops.chongqing.xiangyun.international.tr@customer.ils.com'),
    (N'DAIRY LAND FOR MILK AND FOOD INDUSTRIES (EL ZAHAR)', N'Representative - Dairy Land For Milk And Food Industries (El Zahar)', N'01020000110', N'ops.dairy.land.for.milk.and.food.indust@customer.ils.com'),
    (N'EGYTRAFO FOR ELECTRICAL INDUSTRIES', N'Representative - Egytrafo For Electrical Industries', N'01020000111', N'ops.egytrafo.for.electrical.industries@customer.ils.com'),
    (N'MIDEA ELECTRICS FRANCE', N'Representative - Midea Electrics France', N'01020000112', N'ops.midea.electrics.france@customer.ils.com'),
    (N'EL LEWA FOR GLASS MANUFACTURING CITY GLASS CO.', N'Representative - El Lewa For Glass Manufacturing City Glass Co.', N'01020000113', N'ops.el.lewa.for.glass.manufacturing.cit@customer.ils.com'),
    (N'ANKO ANADOLU KONTEYNER A.S.', N'Representative - Anko Anadolu Konteyner A.S.', N'01020000114', N'ops.anko.anadolu.konteyner.a.s@customer.ils.com'),
    (N'WUXI AUTOWELL SUPPLY CHAIN MANAGEMENT CO.,LTD', N'Representative - Wuxi Autowell Supply Chain Management Co.,Ltd', N'01020000115', N'ops.wuxi.autowell.supply.chain.manageme@customer.ils.com'),
    (N'KEMET PRECIOUS METAL " KPM "', N'Representative - Kemet Precious Metal " Kpm "', N'01020000116', N'ops.kemet.precious.metal.kpm@customer.ils.com'),
    (N'JADEELA STEEL INDUSTRIES', N'Representative - Jadeela Steel Industries', N'01020000117', N'ops.jadeela.steel.industries@customer.ils.com'),
    (N'OPT LOGISTICS CO.,LTD', N'Representative - Opt Logistics Co.,Ltd', N'01020000118', N'ops.opt.logistics.co.ltd@customer.ils.com'),
    (N'MISR CHEMICALS COMPANY TAKLA AND CO.', N'Representative - Misr Chemicals Company Takla And Co.', N'01020000119', N'ops.misr.chemicals.company.takla.and.co@customer.ils.com'),
    (N'FRENCH TRADE FOR IMPORT AND EXPORT', N'Representative - French Trade For Import And Export', N'01020000120', N'ops.french.trade.for.import.and.export@customer.ils.com'),
    (N'HANDA EGYPT TEXTILE S.A.E', N'Representative - Handa Egypt Textile S.A.E', N'01020000121', N'ops.handa.egypt.textile.s.a.e@customer.ils.com'),
    (N'WW LOGISTICIANS PVT LTD', N'Representative - Ww Logisticians Pvt Ltd', N'01020000122', N'ops.ww.logisticians.pvt.ltd@customer.ils.com'),
    (N'ARAB COMPANY FOR ELECTRICAL INDUSTRIES ( ACEI )', N'Representative - Arab Company For Electrical Industries ( Acei )', N'01020000123', N'ops.arab.company.for.electrical.industr@customer.ils.com'),
    (N'DIAMOND MARINE FOR SHIPPING AGENCIES', N'Representative - Diamond Marine For Shipping Agencies', N'01020000124', N'ops.diamond.marine.for.shipping.agencie@customer.ils.com'),
    (N'AYMAN RAGAB SAYED MAHMOUD', N'Representative - Ayman Ragab Sayed Mahmoud', N'01020000125', N'ops.ayman.ragab.sayed.mahmoud@customer.ils.com'),
    (N'COATS EGYPT', N'Representative - Coats Egypt', N'01020000126', N'ops.coats.egypt@customer.ils.com'),
    (N'MARIB INTERNATIONAL GARMENTS COMPANY PRIVATE FREE ZONE', N'Representative - Marib International Garments Company Private Free Zone', N'01020000127', N'ops.marib.international.garments.compan@customer.ils.com'),
    (N'XUHENG EGYPT SUPPLY CHAINMANAGEMENT CO LTD', N'Representative - Xuheng Egypt Supply Chainmanagement Co Ltd', N'01020000128', N'ops.xuheng.egypt.supply.chainmanagement@customer.ils.com'),
    (N'BASHAER ALAFEIA FOR TRADING CO', N'Representative - Bashaer Alafeia For Trading Co', N'01020000129', N'ops.bashaer.alafeia.for.trading.co@customer.ils.com'),
    (N'SHENZHEN MATRIX IMPORTS&EXPORTS CO.,LTD', N'Representative - Shenzhen Matrix Imports&Exports Co.,Ltd', N'01020000130', N'ops.shenzhen.matrix.imports.exports.co@customer.ils.com'),
    (N'TMI CONTAINER LINE SDN. BHD.', N'Representative - Tmi Container Line Sdn. Bhd.', N'01020000131', N'ops.tmi.container.line.sdn.bhd@customer.ils.com'),
    (N'MULTI SHIPPING SOLUTIONS', N'Representative - Multi Shipping Solutions', N'01020000132', N'ops.multi.shipping.solutions@customer.ils.com'),
    (N'EXCELLENT SERVICE FREIGHTERS LIMITED, (A 20CUBE GROUP COMPANY)', N'Representative - Excellent Service Freighters Limited, (A 20Cube Group Company)', N'01020000133', N'ops.excellent.service.freighters.limite@customer.ils.com'),
    (N'GUANGDONG WELLING MOTOR MANUFACTURING CO.', N'Representative - Guangdong Welling Motor Manufacturing Co.', N'01020000134', N'ops.guangdong.welling.motor.manufacturi@customer.ils.com'),
    (N'NAGA GARMENTS. CO.', N'Representative - Naga Garments. Co.', N'01020000135', N'ops.naga.garments.co@customer.ils.com'),
    (N'SHIV SHIPPING & LOGISTICS', N'Representative - Shiv Shipping & Logistics', N'01020000136', N'ops.shiv.shipping.logistics@customer.ils.com'),
    (N'HK DIAMOND TRADE LIMITED', N'Representative - Hk Diamond Trade Limited', N'01020000137', N'ops.hk.diamond.trade.limited@customer.ils.com'),
    (N'G.M STONE CO.', N'Representative - G.M Stone Co.', N'01020000138', N'ops.g.m.stone.co@customer.ils.com'),
    (N'AUROTRANS LOJISTIK HIZMETLERI TICARET LIMITED SIRKETI', N'Representative - Aurotrans Lojistik Hizmetleri Ticaret Limited Sirketi', N'01020000139', N'ops.aurotrans.lojistik.hizmetleri.ticar@customer.ils.com'),
    (N'FIRST DEVELOPED COAST COMPANY', N'Representative - First Developed Coast Company', N'01020000140', N'ops.first.developed.coast.company@customer.ils.com'),
    (N'EL SEWEDY ELECTRIC FOR ELECTRICAL PRODUCTS', N'Representative - El Sewedy Electric For Electrical Products', N'01020000141', N'ops.el.sewedy.electric.for.electrical.p@customer.ils.com'),
    (N'MEDDY TRADE FOR MULTI ACTIVITIES CO LTD', N'Representative - Meddy Trade For Multi Activities Co Ltd', N'01020000142', N'ops.meddy.trade.for.multi.activities.co@customer.ils.com'),
    (N'NEW ELSABEEL CO. FOR POULTRY PRODUCTION', N'Representative - New Elsabeel Co. For Poultry Production', N'01020000143', N'ops.new.elsabeel.co.for.poultry.product@customer.ils.com'),
    (N'ISKRAEMECO ENERGY MEASUREMENT', N'Representative - Iskraemeco Energy Measurement', N'01020000144', N'ops.iskraemeco.energy.measurement@customer.ils.com'),
    (N'SUEZ STEEL CO', N'Representative - Suez Steel Co', N'01020000145', N'ops.suez.steel.co@customer.ils.com'),
    (N'EL ARABY CO. FOR ELECTRICAL AND ELECTRONIC APPLIANCES INDUSTRY', N'Representative - El Araby Co. For Electrical And Electronic Appliances Industry', N'01020000146', N'ops.el.araby.co.for.electrical.and.elec@customer.ils.com'),
    (N'BUZJANY CONSTRUCTION', N'Representative - Buzjany Construction', N'01020000147', N'ops.buzjany.construction@customer.ils.com'),
    (N'CHINA RAILWAY 20BUREAUGROUP CORPORATION', N'Representative - China Railway 20Bureaugroup Corporation', N'01020000148', N'ops.china.railway.20bureaugroup.corpora@customer.ils.com'),
    (N'MARMO STAR FOR MARBLE & GRANITE', N'Representative - Marmo Star For Marble & Granite', N'01020000149', N'ops.marmo.star.for.marble.granite@customer.ils.com'),
    (N'EGYPT STONE FOR MINING AND SUPPLIES', N'Representative - Egypt Stone For Mining And Supplies', N'01020000150', N'ops.egypt.stone.for.mining.and.supplies@customer.ils.com'),
    (N'ORASCOM FREE ZONE', N'Representative - Orascom Free Zone', N'01020000151', N'ops.orascom.free.zone@customer.ils.com'),
    (N'AGTHIA GROUP EGYPT LLC', N'Representative - Agthia Group Egypt Llc', N'01020000152', N'ops.agthia.group.egypt.llc@customer.ils.com'),
    (N'UNIFREIGHT GLOBAL LOGISTIC', N'Representative - Unifreight Global Logistic', N'01020000153', N'ops.unifreight.global.logistic@customer.ils.com'),
    (N'FREE PL FOR DIGITAL SOLUTIONS', N'Representative - Free Pl For Digital Solutions', N'01020000154', N'ops.free.pl.for.digital.solutions@customer.ils.com'),
    (N'ELKHALIL CO. FOR IMPORT, TRADING, & MANFACTURING OF WOOD', N'Representative - Elkhalil Co. For Import, Trading, & Manfacturing Of Wood', N'01020000155', N'ops.elkhalil.co.for.import.trading.manf@customer.ils.com'),
    (N'CHECKPOINT CHECKNET ETIKET LTD STI', N'Representative - Checkpoint Checknet Etiket Ltd Sti', N'01020000156', N'ops.checkpoint.checknet.etiket.ltd.sti@customer.ils.com'),
    (N'MANSOURA UNIVERSITY FACULTY OF SCIENCE, GEOLOGY DEPARTMENT', N'Representative - Mansoura University Faculty Of Science, Geology Department', N'01020000157', N'ops.mansoura.university.faculty.of.scie@customer.ils.com'),
    (N'GUANGZHOU MIDEA HUALING REFRIGERATOR CO.,LTD', N'Representative - Guangzhou Midea Hualing Refrigerator Co.,Ltd', N'01020000158', N'ops.guangzhou.midea.hualing.refrigerato@customer.ils.com'),
    (N'GLOBAL LOGISTIC SOLUTIONS PAKISTAN', N'Representative - Global Logistic Solutions Pakistan', N'01020000159', N'ops.global.logistic.solutions.pakistan@customer.ils.com'),
    (N'ETISALAT MISR', N'Representative - Etisalat Misr', N'01020000160', N'ops.etisalat.misr@customer.ils.com'),
    (N'MASR AFRICA FOR SHIPPING AND INTERNATIONAL TRANSPORTATION', N'Representative - Masr Africa For Shipping And International Transportation', N'01020000161', N'ops.masr.africa.for.shipping.and.intern@customer.ils.com'),
    (N'MIDEA KITCHEN AND BATHROOM APPLIANCE MANNFACTURALNG CO.,LTD', N'Representative - Midea Kitchen And Bathroom Appliance Mannfacturalng Co.,Ltd', N'01020000162', N'ops.midea.kitchen.and.bathroom.applianc@customer.ils.com'),
    (N'PT. OPTIMUS PRIMA LOGISTIK', N'Representative - Pt. Optimus Prima Logistik', N'01020000163', N'ops.pt.optimus.prima.logistik@customer.ils.com'),
    (N'ALSHARQ KAAN TEXTILE', N'Representative - Alsharq Kaan Textile', N'01020000164', N'ops.alsharq.kaan.textile@customer.ils.com'),
    (N'ICHIBAN GLOBAL COMPANY LIMITED', N'Representative - Ichiban Global Company Limited', N'01020000165', N'ops.ichiban.global.company.limited@customer.ils.com'),
    (N'EMIRATES LOGISTICS', N'Representative - Emirates Logistics', N'01020000166', N'ops.emirates.logistics@customer.ils.com'),
    (N'MANTRAC FREE ZONE', N'Representative - Mantrac Free Zone', N'01020000167', N'ops.mantrac.free.zone@customer.ils.com'),
    (N'RTR FOR CUSTOMS SERVICES', N'Representative - Rtr For Customs Services', N'01020000168', N'ops.rtr.for.customs.services@customer.ils.com'),
    (N'SML ETIKET VE AKS.SAN. TIC.PAZ.LTD.STL.', N'Representative - Sml Etiket Ve Aks.San. Tic.Paz.Ltd.Stl.', N'01020000169', N'ops.sml.etiket.ve.aks.san.tic.paz.ltd.s@customer.ils.com'),
    (N'KINDCHEM NANJING CO.LTD', N'Representative - Kindchem Nanjing Co.Ltd', N'01020000170', N'ops.kindchem.nanjing.co.ltd@customer.ils.com'),
    (N'WUXI HUALITE METAL PLASTIC PRODUCTS CO', N'Representative - Wuxi Hualite Metal Plastic Products Co', N'01020000171', N'ops.wuxi.hualite.metal.plastic.products@customer.ils.com'),
    (N'TECHNOMAX COMPANY FOR IMPORT AND EXPORT', N'Representative - Technomax Company For Import And Export', N'01020000172', N'ops.technomax.company.for.import.and.ex@customer.ils.com'),
    (N'EUROPEAN FOR INVESTMENTS', N'Representative - European For Investments', N'01020000173', N'ops.european.for.investments@customer.ils.com'),
    (N'MILSPED ITALIA SRL', N'Representative - Milsped Italia Srl', N'01020000174', N'ops.milsped.italia.srl@customer.ils.com'),
    (N'AL BEIRUTI FOR TOURISM INVESTMENT', N'Representative - Al Beiruti For Tourism Investment', N'01020000175', N'ops.al.beiruti.for.tourism.investment@customer.ils.com'),
    (N'EGYPTIAN ITALIAN CO', N'Representative - Egyptian Italian Co', N'01020000176', N'ops.egyptian.italian.co@customer.ils.com'),
    (N'SHENZHEN LONGDA INDUSTRIAL EQUIPMENT', N'Representative - Shenzhen Longda Industrial Equipment', N'01020000177', N'ops.shenzhen.longda.industrial.equipmen@customer.ils.com'),
    (N'FOSHAN NUOBEIAN COMMUNICATION LTD', N'Representative - Foshan Nuobeian Communication Ltd', N'01020000178', N'ops.foshan.nuobeian.communication.ltd@customer.ils.com'),
    (N'MAC CARPET', N'Representative - Mac Carpet', N'01020000179', N'ops.mac.carpet@customer.ils.com'),
    (N'INTERNATIONAL CONTROL SYSTEMS', N'Representative - International Control Systems', N'01020000180', N'ops.international.control.systems@customer.ils.com'),
    (N'BILAL HAKSEVER AND CO', N'Representative - Bilal Haksever And Co', N'01020000181', N'ops.bilal.haksever.and.co@customer.ils.com'),
    (N'TAGHLEEF INDUSTRIES S.A.E', N'Representative - Taghleef Industries S.A.E', N'01020000182', N'ops.taghleef.industries.s.a.e@customer.ils.com'),
    (N'WAD ELFAKY FOR IMPORT & EXPORT', N'Representative - Wad Elfaky For Import & Export', N'01020000183', N'ops.wad.elfaky.for.import.export@customer.ils.com'),
    (N'WADI ALFAKY FOR IMPORT & EXPORT', N'Representative - Wadi Alfaky For Import & Export', N'01020000184', N'ops.wadi.alfaky.for.import.export@customer.ils.com'),
    (N'AL AHRAM FOR PLASTIC MANUFACTURING', N'Representative - Al Ahram For Plastic Manufacturing', N'01020000185', N'ops.al.ahram.for.plastic.manufacturing@customer.ils.com'),
    (N'WAD ALFAKY FOR IMPORT AND EXPORT', N'Representative - Wad Alfaky For Import And Export', N'01020000186', N'ops.wad.alfaky.for.import.and.export@customer.ils.com'),
    (N'MAMIS TRADING ENTERPRISES POR', N'Representative - Mamis Trading Enterprises Por', N'01020000187', N'ops.mamis.trading.enterprises.por@customer.ils.com'),
    (N'ALQASWA FOR IMPORT AND EXPORT', N'Representative - Alqaswa For Import And Export', N'01020000188', N'ops.alqaswa.for.import.and.export@customer.ils.com'),
    (N'AL TAREEK ALAKHDER FOR TRADING', N'Representative - Al Tareek Alakhder For Trading', N'01020000189', N'ops.al.tareek.alakhder.for.trading@customer.ils.com'),
    (N'NINGBO JIANZHOU IMP. & EXP. CO.,LTD.', N'Representative - Ningbo Jianzhou Imp. & Exp. Co.,Ltd.', N'01020000190', N'ops.ningbo.jianzhou.imp.exp.co.ltd@customer.ils.com'),
    (N'SPRING CARGO', N'Representative - Spring Cargo', N'01020000191', N'ops.spring.cargo@customer.ils.com'),
    (N'TERMOPLAST POLLYPACKS ITALY (INDIA) PRIVETA LIMITED', N'Representative - Termoplast Pollypacks Italy (India) Priveta Limited', N'01020000192', N'ops.termoplast.pollypacks.italy.india.p@customer.ils.com'),
    (N'NINGBO YUANCHENG INTERNATIONAL LOGISTICS CO., LTD', N'Representative - Ningbo Yuancheng International Logistics Co., Ltd', N'01020000193', N'ops.ningbo.yuancheng.international.logi@customer.ils.com'),
    (N'EGYPTIAN TEXTILES FOR DYEING AND FINISHING', N'Representative - Egyptian Textiles For Dyeing And Finishing', N'01020000194', N'ops.egyptian.textiles.for.dyeing.and.fi@customer.ils.com'),
    (N'EGYPT COMPUTER', N'Representative - Egypt Computer', N'01020000195', N'ops.egypt.computer@customer.ils.com'),
    (N'ELNAHAR FOR MARBLE&GRANITE CO.', N'Representative - Elnahar For Marble&Granite Co.', N'01020000196', N'ops.elnahar.for.marble.granite.co@customer.ils.com'),
    (N'ULTRA FOR MEDICAL PRODUCTS CO. (ULTRAMED)', N'Representative - Ultra For Medical Products Co. (Ultramed)', N'01020000197', N'ops.ultra.for.medical.products.co.ultra@customer.ils.com'),
    (N'SESCO TRANS FOR DEVELOPED LOGISTICS', N'Representative - Sesco Trans For Developed Logistics', N'01020000198', N'ops.sesco.trans.for.developed.logistics@customer.ils.com'),
    (N'GIVAUDAN EGYPT SAE', N'Representative - Givaudan Egypt Sae', N'01020000199', N'ops.givaudan.egypt.sae@customer.ils.com'),
    (N'SUN RISE', N'Representative - Sun Rise', N'01020000200', N'ops.sun.rise@customer.ils.com'),
    (N'MASTER LINE TEXTILE INDUSTRIES', N'Representative - Master Line Textile Industries', N'01020000201', N'ops.master.line.textile.industries@customer.ils.com'),
    (N'SEVEN STARS SHIP INC', N'Representative - Seven Stars Ship Inc', N'01020000202', N'ops.seven.stars.ship.inc@customer.ils.com'),
    (N'EJAD FOR ENGINEERING SYSTEMS', N'Representative - Ejad For Engineering Systems', N'01020000203', N'ops.ejad.for.engineering.systems@customer.ils.com'),
    (N'SHENZHEN DIGITAL LIFE ELECTRIC COMPANY LTD ANNA CHEN', N'Representative - Shenzhen Digital Life Electric Company Ltd Anna Chen', N'01020000204', N'ops.shenzhen.digital.life.electric.comp@customer.ils.com'),
    (N'ZHEJIANG BROTHER UNITEDTRADING CO.,LTD.', N'Representative - Zhejiang Brother Unitedtrading Co.,Ltd.', N'01020000205', N'ops.zhejiang.brother.unitedtrading.co.l@customer.ils.com'),
    (N'INCOME EGYPT CO', N'Representative - Income Egypt Co', N'01020000206', N'ops.income.egypt.co@customer.ils.com'),
    (N'ARABCO INTERNATIONAL GROUP', N'Representative - Arabco International Group', N'01020000207', N'ops.arabco.international.group@customer.ils.com'),
    (N'FUJIAN SBS ZIPPER SCIENCE AND TECHNOLOGY CO .,LTD', N'Representative - Fujian Sbs Zipper Science And Technology Co .,Ltd', N'01020000208', N'ops.fujian.sbs.zipper.science.and.techn@customer.ils.com'),
    (N'ELZAMEK COMPANY FOR IMPORT & EXPORT', N'Representative - Elzamek Company For Import & Export', N'01020000209', N'ops.elzamek.company.for.import.export@customer.ils.com'),
    (N'ASCOM CARBONATES AND CHEMICALS MANUFACTURING', N'Representative - Ascom Carbonates And Chemicals Manufacturing', N'01020000210', N'ops.ascom.carbonates.and.chemicals.manu@customer.ils.com'),
    (N'SIBELCO ITALIA S.P.A.', N'Representative - Sibelco Italia S.P.A.', N'01020000211', N'ops.sibelco.italia.s.p.a@customer.ils.com'),
    (N'TEKNOFLUOR SRL', N'Representative - Teknofluor Srl', N'01020000212', N'ops.teknofluor.srl@customer.ils.com'),
    (N'EASTERN ELECTRONICS', N'Representative - Eastern Electronics', N'01020000213', N'ops.eastern.electronics@customer.ils.com'),
    (N'EL ARABY CO. FOR AIR CONDITIONING AND COOLING INDUSTRY', N'Representative - El Araby Co. For Air Conditioning And Cooling Industry', N'01020000214', N'ops.el.araby.co.for.air.conditioning.an@customer.ils.com'),
    (N'MAGIC BAG CO.', N'Representative - Magic Bag Co.', N'01020000215', N'ops.magic.bag.co@customer.ils.com'),
    (N'NATURALE', N'Representative - Naturale', N'01020000216', N'ops.naturale@customer.ils.com'),
    (N'SHUNDE XUANTONG IMPORT AND EXPORT TRADE CO., LTD.', N'Representative - Shunde Xuantong Import And Export Trade Co., Ltd.', N'01020000217', N'ops.shunde.xuantong.import.and.export.t@customer.ils.com'),
    (N'CONTA-CLIP GMBH * POSTFACH 51 * 33155 HÖVELHOF', N'Representative - Conta-Clip Gmbh * Postfach 51 * 33155 Hövelhof', N'01020000218', N'ops.conta.clip.gmbh.postfach.51.33155.h@customer.ils.com'),
    (N'RIO PACK CO', N'Representative - Rio Pack Co', N'01020000219', N'ops.rio.pack.co@customer.ils.com'),
    (N'ICON FOR INSULATED PANELS', N'Representative - Icon For Insulated Panels', N'01020000220', N'ops.icon.for.insulated.panels@customer.ils.com'),
    (N'MIDEA ELECTRICS ARABIA LTD', N'Representative - Midea Electrics Arabia Ltd', N'01020000221', N'ops.midea.electrics.arabia.ltd@customer.ils.com'),
    (N'EL ARABY CO FOR COOKING APPLIANCES INDUSTRY', N'Representative - El Araby Co For Cooking Appliances Industry', N'01020000222', N'ops.el.araby.co.for.cooking.appliances@customer.ils.com'),
    (N'VIRTUE FOR MANUFACTURING PLASTIC PRODUCTS JSC', N'Representative - Virtue For Manufacturing Plastic Products Jsc', N'01020000223', N'ops.virtue.for.manufacturing.plastic.pr@customer.ils.com'),
    (N'ELITE SOLAR INC', N'Representative - Elite Solar Inc', N'01020000224', N'ops.elite.solar.inc@customer.ils.com'),
    (N'FUTOP SOLAR EPZ LIMITED', N'Representative - Futop Solar Epz Limited', N'01020000225', N'ops.futop.solar.epz.limited@customer.ils.com'),
    (N'SCHEMAS INTEGRATED ENGINEERING SOLUTION "SIESCOM"', N'Representative - Schemas Integrated Engineering Solution "Siescom"', N'01020000226', N'ops.schemas.integrated.engineering.solu@customer.ils.com'),
    (N'DAHAB COMPANY FOR IMPORT&EXPORT', N'Representative - Dahab Company For Import&Export', N'01020000227', N'ops.dahab.company.for.import.export@customer.ils.com'),
    (N'FU FENG PACKING TECNOLIGY CO. LTD', N'Representative - Fu Feng Packing Tecnoligy Co. Ltd', N'01020000228', N'ops.fu.feng.packing.tecnoligy.co.ltd@customer.ils.com'),
    (N'ELITE SOLAR SUEZ TECHNOLOGY COMPANY LIMITED', N'Representative - Elite Solar Suez Technology Company Limited', N'01020000229', N'ops.elite.solar.suez.technology.company@customer.ils.com'),
    (N'QINGDAO DSI LOGISTIC CO ., LTD', N'Representative - Qingdao Dsi Logistic Co ., Ltd', N'01020000230', N'ops.qingdao.dsi.logistic.co.ltd@customer.ils.com')
) AS source ([CompanyName], [OwnerName], [OwnerPhone], [OwnerEmail])
ON target.[CompanyName] = source.[CompanyName]
WHEN MATCHED THEN
    UPDATE SET target.[OwnerName] = source.[OwnerName], target.[OwnerPhone] = source.[OwnerPhone], target.[OwnerEmail] = source.[OwnerEmail]
WHEN NOT MATCHED THEN
    INSERT ([Id], [OwnerName], [CompanyName], [OwnerPhone], [OwnerEmail], [Industry], [OwnerUserId], [Status], [IsDeleted])
    VALUES (NEWID(), source.[OwnerName], source.[CompanyName], source.[OwnerPhone], source.[OwnerEmail], NULL, NEWID(), N'Active', 0);
GO

-- Seed matching Identity Users if Users table exists
IF OBJECT_ID(N'[Users]') IS NOT NULL
BEGIN
    PRINT 'Seeding corresponding Identity Users...';
    MERGE INTO [Users] AS target
    USING (
        SELECT [OwnerUserId] AS [Id], [OwnerEmail] AS [Email], [OwnerPhone] AS [PhoneNumber]
        FROM [Customers]
        WHERE [OwnerEmail] LIKE '%@customer.ils.com'
    ) AS source
    ON target.[Id] = source.[Id]
    WHEN NOT MATCHED THEN
        INSERT ([Id], [Kind], [IsActive], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [SecurityStamp], [ConcurrencyStamp])
        VALUES (source.[Id], 1, 1, source.[Email], UPPER(source.[Email]), source.[Email], UPPER(source.[Email]), 1, source.[PhoneNumber], 0, 0, 1, 0, NEWID(), NEWID());
END;
GO

