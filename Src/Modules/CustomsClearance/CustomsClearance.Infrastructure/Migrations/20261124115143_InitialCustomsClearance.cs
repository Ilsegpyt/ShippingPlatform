using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomsClearance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCustomsClearance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "customs");

            migrationBuilder.CreateTable(
                name: "CustomsShipments",
                schema: "customs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ShipmentNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LatestUpdate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsShipments", x => x.Id);
                    table.CheckConstraint("CK_CustomsShipments_ReportType", "[ReportType] IN ('OceanImport', 'OceanExport', 'Domestic')");
                    table.CheckConstraint("CK_CustomsShipments_Status", "[Status] IN ('Open', 'Closed')");
                });

            migrationBuilder.CreateTable(
                name: "CustomsContainers",
                schema: "customs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<short>(type: "smallint", nullable: false),
                    ContainerType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsContainers", x => x.Id);
                    table.CheckConstraint("CK_CustomsContainers_ContainerType", "[ContainerType] IN ('20', '40', '40 HC', '45 HC')");
                    table.CheckConstraint("CK_CustomsContainers_Ordinal", "[Ordinal] >= 1");
                    table.ForeignKey(
                        name: "FK_CustomsContainers_CustomsShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "customs",
                        principalTable: "CustomsShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomsUpdateLogs",
                schema: "customs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsUpdateLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomsUpdateLogs_CustomsShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "customs",
                        principalTable: "CustomsShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DomesticImportDetails",
                schema: "customs",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommercialInvoiceNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NumberOfPieces = table.Column<int>(type: "int", nullable: false),
                    MissingDocs = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DateOfReceivingMissingDocs = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomesticImportDetails", x => x.ShipmentId);
                    table.CheckConstraint("CK_DomesticImportDetails_NumberOfPieces", "[NumberOfPieces] >= 1");
                    table.ForeignKey(
                        name: "FK_DomesticImportDetails_CustomsShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "customs",
                        principalTable: "CustomsShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OceanExportDetails",
                schema: "customs",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClearanceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomsArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumberOfContainers = table.Column<int>(type: "int", nullable: false),
                    ContainerType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Pol = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Pod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BookingNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CommercialInvoiceNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Cutoff = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoadingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CertificateDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DateOfCoo = table.Column<DateOnly>(type: "date", nullable: true),
                    DateOfReceivingForm13 = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OceanExportDetails", x => x.ShipmentId);
                    table.CheckConstraint("CK_OceanExportDetails_ClearanceType", "[ClearanceType] IN ('Certificate', 'Bosla')");
                    table.CheckConstraint("CK_OceanExportDetails_ContainerType", "[ContainerType] IS NULL OR [ContainerType] IN ('20', '40', '40 HC', '45 HC')");
                    table.CheckConstraint("CK_OceanExportDetails_CustomsArea", "[ClearanceType] = 'Bosla' OR ([ClearanceType] = 'Certificate' AND ([CustomsArea] IS NULL OR [CustomsArea] = ''))");
                    table.CheckConstraint("CK_OceanExportDetails_NumberOfContainers", "[NumberOfContainers] BETWEEN 1 AND 10000");
                    table.ForeignKey(
                        name: "FK_OceanExportDetails_CustomsShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "customs",
                        principalTable: "CustomsShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OceanImportDetails",
                schema: "customs",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClearanceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AcidNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: true),
                    Mbl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Hbl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CustomsDeclarations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumberOfContainers = table.Column<int>(type: "int", nullable: true),
                    ContainerType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ShippingLine = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Commodity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Pod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Terminal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EtaDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AtaDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FreeTime = table.Column<int>(type: "int", nullable: true),
                    DateOfFreeTimeEnd = table.Column<DateOnly>(type: "date", nullable: true),
                    DoDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MissingDocs = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InspectionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CurrentStage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OceanImportDetails", x => x.ShipmentId);
                    table.CheckConstraint("CK_OceanImportDetails_AcidNumber", "[AcidNumber] IS NULL OR (LEN([AcidNumber]) = 19 AND [AcidNumber] NOT LIKE '%[^0-9]%')");
                    table.CheckConstraint("CK_OceanImportDetails_ClearanceType", "[ClearanceType] IN ('Import', 'Transit')");
                    table.CheckConstraint("CK_OceanImportDetails_ContainerType", "[ContainerType] IS NULL OR [ContainerType] IN ('20', '40', '40 HC', '45 HC')");
                    table.CheckConstraint("CK_OceanImportDetails_FreeTime", "[FreeTime] IS NULL OR [FreeTime] >= 0");
                    table.CheckConstraint("CK_OceanImportDetails_NumberOfContainers", "[NumberOfContainers] IS NULL OR [NumberOfContainers] BETWEEN 1 AND 10000");
                    table.ForeignKey(
                        name: "FK_OceanImportDetails_CustomsShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "customs",
                        principalTable: "CustomsShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomsContainers_ShipmentId_Ordinal",
                schema: "customs",
                table: "CustomsContainers",
                columns: new[] { "ShipmentId", "Ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomsShipments_ClientId",
                schema: "customs",
                table: "CustomsShipments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsShipments_ReportType_Status",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "ReportType", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_CustomsShipments_ReportType_CertificateNumber",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "ReportType", "CertificateNumber" },
                unique: true,
                filter: "[CertificateNumber] IS NOT NULL AND [CertificateNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_CustomsShipments_ReportType_ShipmentNumber",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "ReportType", "ShipmentNumber" },
                unique: true,
                filter: "[ShipmentNumber] IS NOT NULL AND [ShipmentNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsUpdateLogs_ShipmentId_UpdatedAtUtc",
                schema: "customs",
                table: "CustomsUpdateLogs",
                columns: new[] { "ShipmentId", "UpdatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_DomesticImportDetails_CommercialInvoiceNumber",
                schema: "customs",
                table: "DomesticImportDetails",
                column: "CommercialInvoiceNumber",
                unique: true,
                filter: "[CommercialInvoiceNumber] IS NOT NULL AND [CommercialInvoiceNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_OceanExportDetails_BookingNumber",
                schema: "customs",
                table: "OceanExportDetails",
                column: "BookingNumber",
                unique: true,
                filter: "[BookingNumber] IS NOT NULL AND [BookingNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_OceanExportDetails_CommercialInvoiceNumber",
                schema: "customs",
                table: "OceanExportDetails",
                column: "CommercialInvoiceNumber",
                unique: true,
                filter: "[CommercialInvoiceNumber] IS NOT NULL AND [CommercialInvoiceNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_OceanImportDetails_AcidNumber",
                schema: "customs",
                table: "OceanImportDetails",
                column: "AcidNumber",
                unique: true,
                filter: "[AcidNumber] IS NOT NULL AND [AcidNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_OceanImportDetails_Hbl",
                schema: "customs",
                table: "OceanImportDetails",
                column: "Hbl",
                unique: true,
                filter: "[Hbl] IS NOT NULL AND [Hbl] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_OceanImportDetails_Mbl",
                schema: "customs",
                table: "OceanImportDetails",
                column: "Mbl",
                unique: true,
                filter: "[Mbl] IS NOT NULL AND [Mbl] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomsContainers",
                schema: "customs");

            migrationBuilder.DropTable(
                name: "CustomsUpdateLogs",
                schema: "customs");

            migrationBuilder.DropTable(
                name: "DomesticImportDetails",
                schema: "customs");

            migrationBuilder.DropTable(
                name: "OceanExportDetails",
                schema: "customs");

            migrationBuilder.DropTable(
                name: "OceanImportDetails",
                schema: "customs");

            migrationBuilder.DropTable(
                name: "CustomsShipments",
                schema: "customs");
        }
    }
}
