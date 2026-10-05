using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMasterDataAuditSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Vehicles");

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
                name: "UpdatedAtUtc",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Transporters");

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
                name: "UpdatedAtUtc",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Transporters");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ShippingLines");

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
                name: "UpdatedAtUtc",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Ports");

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
                name: "UpdatedAtUtc",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Drivers");

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
                name: "UpdatedAtUtc",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Drivers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Vehicles",
                type: "nvarchar(max)",
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
                name: "UpdatedAtUtc",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Transporters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Transporters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Transporters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Transporters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Transporters",
                type: "nvarchar(max)",
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
                name: "UpdatedAtUtc",
                table: "Transporters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "Transporters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Transporters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "ShippingLines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "ShippingLines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ShippingLines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "ShippingLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "ShippingLines",
                type: "nvarchar(max)",
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
                name: "UpdatedAtUtc",
                table: "ShippingLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "ShippingLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "ShippingLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Ports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Ports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Ports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Ports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Ports",
                type: "nvarchar(max)",
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
                name: "UpdatedAtUtc",
                table: "Ports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "Ports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Ports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Drivers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "Drivers",
                type: "nvarchar(max)",
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
                name: "UpdatedAtUtc",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
