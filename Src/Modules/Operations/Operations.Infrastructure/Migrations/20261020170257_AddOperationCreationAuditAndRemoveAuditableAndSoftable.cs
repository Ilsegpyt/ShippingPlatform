using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationCreationAuditAndRemoveAuditableAndSoftable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Operations_OperationType_DuplicateCheckKey",
                table: "Operations");

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
                name: "UpdatedAtUtc",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ExportDetails");

            migrationBuilder.CreateIndex(
                name: "UX_Operations_OperationType_DuplicateCheckKey",
                table: "Operations",
                columns: new[] { "OperationType", "DuplicateCheckKey" },
                unique: true,
                filter: "[DuplicateCheckKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Operations_OperationType_DuplicateCheckKey",
                table: "Operations");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Operations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "Operations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Operations",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "OperationContainers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "OperationContainers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "OperationContainers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "OperationContainers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "OperationContainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "OperationContainers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OperationContainers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "OperationContainers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "OperationContainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "OperationContainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "ImportDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "ImportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ImportDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "ImportDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "ImportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ImportDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ImportDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "ImportDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "ImportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "ImportDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "ExportDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "ExportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ExportDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "ExportDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "ExportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ExportDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExportDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "ExportDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "ExportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "ExportDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_Operations_OperationType_DuplicateCheckKey",
                table: "Operations",
                columns: new[] { "OperationType", "DuplicateCheckKey" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [DuplicateCheckKey] IS NOT NULL");
        }
    }
}
