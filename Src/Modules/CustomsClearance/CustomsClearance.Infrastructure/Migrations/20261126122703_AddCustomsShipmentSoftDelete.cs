using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomsClearance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomsShipmentSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomsShipments_ReportType_Status",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropIndex(
                name: "UX_CustomsShipments_ReportType_CertificateNumber",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropIndex(
                name: "UX_CustomsShipments_ReportType_ShipmentNumber",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "customs",
                table: "CustomsShipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                schema: "customs",
                table: "CustomsShipments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                schema: "customs",
                table: "CustomsShipments",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "customs",
                table: "CustomsShipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CustomsShipments_Inactive_ReportType_DeletedAtUtc",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "IsDeleted", "ReportType", "DeletedAtUtc" },
                descending: new[] { false, false, true },
                filter: "[IsDeleted] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsShipments_IsDeleted_ReportType_Status",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "IsDeleted", "ReportType", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_CustomsShipments_ReportType_CertificateNumber",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "ReportType", "CertificateNumber" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [CertificateNumber] IS NOT NULL AND [CertificateNumber] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_CustomsShipments_ReportType_ShipmentNumber",
                schema: "customs",
                table: "CustomsShipments",
                columns: new[] { "ReportType", "ShipmentNumber" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [ShipmentNumber] IS NOT NULL AND [ShipmentNumber] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomsShipments_Inactive_ReportType_DeletedAtUtc",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropIndex(
                name: "IX_CustomsShipments_IsDeleted_ReportType_Status",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropIndex(
                name: "UX_CustomsShipments_ReportType_CertificateNumber",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropIndex(
                name: "UX_CustomsShipments_ReportType_ShipmentNumber",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "customs",
                table: "CustomsShipments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "customs",
                table: "CustomsShipments");

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
        }
    }
}
