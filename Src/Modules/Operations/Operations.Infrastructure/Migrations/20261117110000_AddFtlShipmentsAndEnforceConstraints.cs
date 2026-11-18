using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFtlShipmentsAndEnforceConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_FtlOperations_Ref",
                table: "FtlOperations");

            migrationBuilder.DropColumn(
                name: "Ref",
                table: "FtlOperations");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfShipments",
                table: "FtlOperations",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ArrivalDate",
                table: "FtlTrucks",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ArrivalTime",
                table: "FtlTrucks",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RequiredArrivalDate",
                table: "FtlTrucks",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "RequiredArrivalTime",
                table: "FtlTrucks",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FtlShipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FtlOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<short>(type: "smallint", nullable: false),
                    ShipmentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MAWB = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HAWB = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClearanceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomsArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Packages = table.Column<int>(type: "int", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    Cbm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Commodity = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Terminal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AOD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InvoicesReceivedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransporterInvoiceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MissingDocs = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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

            migrationBuilder.AddColumn<Guid>(
                name: "FtlShipmentId",
                table: "FtlOperationTrips",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FtlShipments_Operation_Ordinal",
                table: "FtlShipments",
                columns: new[] { "FtlOperationId", "Ordinal" });

            migrationBuilder.CreateIndex(
                name: "IX_FtlShipments_ShipmentNumber",
                table: "FtlShipments",
                column: "ShipmentNumber",
                filter: "[ShipmentNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FtlShipments_HAWB",
                table: "FtlShipments",
                column: "HAWB",
                filter: "[HAWB] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FtlShipments_MAWB_HAWB",
                table: "FtlShipments",
                columns: new[] { "MAWB", "HAWB" },
                filter: "[MAWB] IS NOT NULL AND [HAWB] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FtlShipments_Cert_ShipmentNumber",
                table: "FtlShipments",
                columns: new[] { "CertificateNumber", "ShipmentNumber" },
                filter: "[CertificateNumber] IS NOT NULL AND [ShipmentNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FtlOperationTrips_FtlShipmentId",
                table: "FtlOperationTrips",
                column: "FtlShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FtlOperationTrips_FtlShipments_FtlShipmentId",
                table: "FtlOperationTrips",
                column: "FtlShipmentId",
                principalTable: "FtlShipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Data Migration: copy legacy data from FtlOperations into FtlShipments & FtlTrucks
            migrationBuilder.Sql(@"
                -- 1. Migrate shipment data
                INSERT INTO [FtlShipments] (
                    [Id], [FtlOperationId], [Ordinal], [ShipmentNumber], [MAWB], [HAWB], [CertificateNumber],
                    [ClearanceType], [ClientId], [ClientName], [CustomsArea], [Packages], [GrossWeight],
                    [Cbm], [Commodity], [Terminal], [Destination], [AOD], [InvoicesReceivedDate]
                )
                SELECT 
                    NEWID(), o.Id, 0, NULL, ISNULL(o.BL, o.BookingNumber), NULL, o.CertificateNumber,
                    o.ClearanceType, o.ClientId, o.ClientName, o.CustomsArea, o.Packages, o.GrossWeight,
                    o.Cbm, o.Commodity, o.Terminal, o.Destination, o.AOD, o.InvoicesReceivedDate
                FROM [FtlOperations] o
                WHERE NOT EXISTS (SELECT 1 FROM [FtlShipments] s WHERE s.FtlOperationId = o.Id);

                -- 2. Migrate arrival dates to FtlTrucks
                UPDATE t
                SET 
                    t.ArrivalDate = ISNULL(t.ArrivalDate, o.ArrivalDate),
                    t.ArrivalTime = ISNULL(t.ArrivalTime, o.ArrivalTime),
                    t.RequiredArrivalDate = ISNULL(t.RequiredArrivalDate, o.RequiredArrivalDate),
                    t.RequiredArrivalTime = ISNULL(t.RequiredArrivalTime, o.RequiredArrivalTime)
                FROM [FtlTrucks] t
                INNER JOIN [FtlOperations] o ON t.FtlOperationId = o.Id
                WHERE t.ArrivalDate IS NULL AND t.RequiredArrivalDate IS NULL;

                -- 3. Link unlinked trips
                UPDATE tr
                SET tr.FtlShipmentId = s.Id
                FROM [FtlOperationTrips] tr
                INNER JOIN [FtlShipments] s ON tr.FtlOperationId = s.FtlOperationId AND s.Ordinal = 0
                WHERE tr.FtlShipmentId IS NULL;
            ");

            // Drop 19 legacy columns from FtlOperations (BL, BookingNumber, ClearanceType, CertificateNumber, Packages, GrossWeight, Cbm, Commodity, Terminal, Destination, AOD, CustomsArea, InvoicesReceivedDate, ClientId, ClientName, ArrivalDate, ArrivalTime, RequiredArrivalDate, RequiredArrivalTime)
            migrationBuilder.DropColumn(name: "ClientId", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "ClientName", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "BL", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "BookingNumber", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "ClearanceType", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "CertificateNumber", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "Packages", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "GrossWeight", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "Cbm", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "Commodity", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "Terminal", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "Destination", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "AOD", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "ArrivalDate", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "ArrivalTime", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "RequiredArrivalDate", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "RequiredArrivalTime", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "CustomsArea", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "InvoicesReceivedDate", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "EnteredByUserId", table: "FtlOperations");
            migrationBuilder.DropColumn(name: "EnteredByName", table: "FtlOperations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FtlOperationTrips_FtlShipments_FtlShipmentId",
                table: "FtlOperationTrips");

            migrationBuilder.DropIndex(
                name: "IX_FtlOperationTrips_FtlShipmentId",
                table: "FtlOperationTrips");

            migrationBuilder.DropColumn(
                name: "FtlShipmentId",
                table: "FtlOperationTrips");

            migrationBuilder.DropTable(
                name: "FtlShipments");

            migrationBuilder.DropColumn(
                name: "RequiredArrivalTime",
                table: "FtlTrucks");

            migrationBuilder.DropColumn(
                name: "RequiredArrivalDate",
                table: "FtlTrucks");

            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "FtlTrucks");

            migrationBuilder.DropColumn(
                name: "ArrivalDate",
                table: "FtlTrucks");

            migrationBuilder.DropColumn(
                name: "NumberOfShipments",
                table: "FtlOperations");

            migrationBuilder.AddColumn<string>(
                name: "Ref",
                table: "FtlOperations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_FtlOperations_Ref",
                table: "FtlOperations",
                column: "Ref",
                unique: true);
        }
    }
}
