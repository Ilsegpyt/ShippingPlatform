SET QUOTED_IDENTIFIER ON;
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
