using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Operations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Operations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Operations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Operations",
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

            migrationBuilder.CreateIndex(
                name: "IX_ExportDetails_PODId",
                table: "ExportDetails",
                column: "PODId");

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
                name: "DeletedAtUtc",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Operations");

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
        }
    }
}
