using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFtlAirportModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Destinations");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Vehicles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Vehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Transporters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Transporters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Transporters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Transporters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "ShippingLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "ShippingLines",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ShippingLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShippingLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Ports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Ports",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Ports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Ports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PODId",
                table: "ExportDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Drivers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Destinations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Destinations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Destinations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Destinations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FtlAirports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlAirports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FtlDrivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DriverNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlDrivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FtlOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ref = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BL = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    BookingNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ClearanceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Packages = table.Column<int>(type: "int", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Cbm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Commodity = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Terminal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AOD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ArrivalTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    RequiredArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RequiredArrivalTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CustomsArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvoicesReceivedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LayoutJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    EnteredByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EnteredByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LockedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LockedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FtlTerminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlTerminals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FtlTransporters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlTransporters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FtlShipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FtlOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<short>(type: "smallint", nullable: false),
                    ShipmentNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HAWB = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Packages = table.Column<int>(type: "int", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Cbm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Commodity = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CustomsArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvoicesReceivedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransporterInvoiceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MissingDocs = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MissingDocsReceivedDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlShipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FtlShipments_FtlOperations_FtlOperationId",
                        column: x => x.FtlOperationId,
                        principalTable: "FtlOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FtlStatusAudits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FtlOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeforeStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AfterStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlStatusAudits", x => x.Id);
                    table.CheckConstraint("CK_FtlStatusAudits_StatusChanged", "[BeforeStatus] <> [AfterStatus]");
                    table.ForeignKey(
                        name: "FK_FtlStatusAudits_FtlOperations_FtlOperationId",
                        column: x => x.FtlOperationId,
                        principalTable: "FtlOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FtlTrucks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FtlOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<short>(type: "smallint", nullable: false),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransporterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TruckType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TruckPlateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DriverMobile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GateInDate = table.Column<DateOnly>(type: "date", nullable: true),
                    GateInTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    GateOutDate = table.Column<DateOnly>(type: "date", nullable: true),
                    GateOutTime = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlTrucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FtlTrucks_FtlOperations_FtlOperationId",
                        column: x => x.FtlOperationId,
                        principalTable: "FtlOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FtlVehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TruckType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FtlTransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FtlVehicles_FtlTransporters_FtlTransporterId",
                        column: x => x.FtlTransporterId,
                        principalTable: "FtlTransporters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FtlOperationTrips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FtlOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<short>(type: "smallint", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FtlShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FtlTruckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LoadStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LoadedPackages = table.Column<int>(type: "int", nullable: true),
                    LoadedWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    LoadedCbm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    FieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlOperationTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FtlOperationTrips_FtlOperations_FtlOperationId",
                        column: x => x.FtlOperationId,
                        principalTable: "FtlOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FtlOperationTrips_FtlShipments_FtlShipmentId",
                        column: x => x.FtlShipmentId,
                        principalTable: "FtlShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FtlOperationTrips_FtlTrucks_FtlTruckId",
                        column: x => x.FtlTruckId,
                        principalTable: "FtlTrucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FtlTruckUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FtlTruckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtlTruckUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FtlTruckUpdates_FtlTrucks_FtlTruckId",
                        column: x => x.FtlTruckId,
                        principalTable: "FtlTrucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportDetails_PODId",
                table: "ExportDetails",
                column: "PODId");

            migrationBuilder.CreateIndex(
                name: "IX_FtlAirports_Name",
                table: "FtlAirports",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FtlDrivers_DriverNumber",
                table: "FtlDrivers",
                column: "DriverNumber",
                unique: true,
                filter: "[DriverNumber] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FtlOperations_Type_Deleted_Status_Created",
                table: "FtlOperations",
                columns: new[] { "OperationType", "IsDeleted", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_FtlOperations_Ref",
                table: "FtlOperations",
                column: "Ref",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FtlOperationTrips_FtlShipmentId",
                table: "FtlOperationTrips",
                column: "FtlShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FtlOperationTrips_FtlTruckId",
                table: "FtlOperationTrips",
                column: "FtlTruckId");

            migrationBuilder.CreateIndex(
                name: "UX_FtlOperationTrips_Operation_Ordinal",
                table: "FtlOperationTrips",
                columns: new[] { "FtlOperationId", "Ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_FtlShipments_Operation_Ordinal",
                table: "FtlShipments",
                columns: new[] { "FtlOperationId", "Ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FtlStatusAudits_FtlOperationId_AtUtc",
                table: "FtlStatusAudits",
                columns: new[] { "FtlOperationId", "AtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_FtlTerminals_Name",
                table: "FtlTerminals",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FtlTransporters_Name",
                table: "FtlTransporters",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UX_FtlTrucks_Operation_Ordinal",
                table: "FtlTrucks",
                columns: new[] { "FtlOperationId", "Ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FtlTruckUpdates_FtlTruckId",
                table: "FtlTruckUpdates",
                column: "FtlTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_FtlVehicles_FtlTransporterId",
                table: "FtlVehicles",
                column: "FtlTransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_FtlVehicles_PlateNumber",
                table: "FtlVehicles",
                column: "PlateNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetails_Ports_PODId",
                table: "ExportDetails",
                column: "PODId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetails_Ports_PODId",
                table: "ExportDetails");

            migrationBuilder.DropTable(
                name: "FtlAirports");

            migrationBuilder.DropTable(
                name: "FtlDrivers");

            migrationBuilder.DropTable(
                name: "FtlOperationTrips");

            migrationBuilder.DropTable(
                name: "FtlStatusAudits");

            migrationBuilder.DropTable(
                name: "FtlTerminals");

            migrationBuilder.DropTable(
                name: "FtlTruckUpdates");

            migrationBuilder.DropTable(
                name: "FtlVehicles");

            migrationBuilder.DropTable(
                name: "FtlShipments");

            migrationBuilder.DropTable(
                name: "FtlTrucks");

            migrationBuilder.DropTable(
                name: "FtlTransporters");

            migrationBuilder.DropTable(
                name: "FtlOperations");

            migrationBuilder.DropIndex(
                name: "IX_ExportDetails_PODId",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "PODId",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Destinations");

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Transporters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Ports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Destinations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
