using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkFclToExistingExportOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FclApprovals_Operations_CreatedExportOperationId",
                table: "FclApprovals");

            migrationBuilder.DropIndex(
                name: "IX_FclApprovals_CreatedExportOperationId",
                table: "FclApprovals");

            migrationBuilder.RenameColumn(
                name: "CreatedExportOperationId",
                table: "FclApprovals",
                newName: "ExportOperationId");

            migrationBuilder.AddColumn<Guid>(
                name: "ExportOperationId",
                table: "FclSuggestions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_ExportOperationId",
                table: "FclSuggestions",
                column: "ExportOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_FclApprovals_ExportOperationId",
                table: "FclApprovals",
                column: "ExportOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_FclApprovals_Operations_ExportOperationId",
                table: "FclApprovals",
                column: "ExportOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FclSuggestions_Operations_ExportOperationId",
                table: "FclSuggestions",
                column: "ExportOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FclApprovals_Operations_ExportOperationId",
                table: "FclApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_FclSuggestions_Operations_ExportOperationId",
                table: "FclSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_FclSuggestions_ExportOperationId",
                table: "FclSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_FclApprovals_ExportOperationId",
                table: "FclApprovals");

            migrationBuilder.DropColumn(
                name: "ExportOperationId",
                table: "FclSuggestions");

            migrationBuilder.RenameColumn(
                name: "ExportOperationId",
                table: "FclApprovals",
                newName: "CreatedExportOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_FclApprovals_CreatedExportOperationId",
                table: "FclApprovals",
                column: "CreatedExportOperationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FclApprovals_Operations_CreatedExportOperationId",
                table: "FclApprovals",
                column: "CreatedExportOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
