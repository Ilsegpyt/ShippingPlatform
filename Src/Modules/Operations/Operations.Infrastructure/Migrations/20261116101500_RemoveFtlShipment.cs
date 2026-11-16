using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFtlShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FtlShipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FtlOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<short>(type: "smallint", nullable: false),
                    Cbm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Commodity = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CustomsArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    HAWB = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    InvoicesReceivedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MissingDocs = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MissingDocsReceivedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Packages = table.Column<int>(type: "int", nullable: true),
                    ShipmentNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TransporterInvoiceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
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
                name: "IX_FtlOperationTrips_FtlShipmentId",
                table: "FtlOperationTrips",
                column: "FtlShipmentId");

            migrationBuilder.CreateIndex(
                name: "UX_FtlShipments_Operation_Ordinal",
                table: "FtlShipments",
                columns: new[] { "FtlOperationId", "Ordinal" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FtlOperationTrips_FtlShipments_FtlShipmentId",
                table: "FtlOperationTrips",
                column: "FtlShipmentId",
                principalTable: "FtlShipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
