using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExportContainerToFclSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "FclSuggestions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "ExportContainerId",
                table: "FclSuggestions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_ExportContainerId",
                table: "FclSuggestions",
                column: "ExportContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FclSuggestions_OperationContainers_ExportContainerId",
                table: "FclSuggestions",
                column: "ExportContainerId",
                principalTable: "OperationContainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FclSuggestions_OperationContainers_ExportContainerId",
                table: "FclSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_FclSuggestions_ExportContainerId",
                table: "FclSuggestions");

            migrationBuilder.DropColumn(
                name: "ExportContainerId",
                table: "FclSuggestions");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "FclSuggestions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
