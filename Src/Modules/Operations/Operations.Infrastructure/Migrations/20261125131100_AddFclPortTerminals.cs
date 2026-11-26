using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFclPortTerminals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TerminalId",
                table: "ImportDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TerminalId",
                table: "ExportDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortTerminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortTerminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortTerminals_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_TerminalId",
                table: "ImportDetails",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportDetails_TerminalId",
                table: "ExportDetails",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_PortTerminals_PortId_Code",
                table: "PortTerminals",
                columns: new[] { "PortId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PortTerminals_PortId_Name",
                table: "PortTerminals",
                columns: new[] { "PortId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetails_PortTerminals_TerminalId",
                table: "ExportDetails",
                column: "TerminalId",
                principalTable: "PortTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportDetails_PortTerminals_TerminalId",
                table: "ImportDetails",
                column: "TerminalId",
                principalTable: "PortTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetails_PortTerminals_TerminalId",
                table: "ExportDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportDetails_PortTerminals_TerminalId",
                table: "ImportDetails");

            migrationBuilder.DropTable(
                name: "PortTerminals");

            migrationBuilder.DropIndex(
                name: "IX_ImportDetails_TerminalId",
                table: "ImportDetails");

            migrationBuilder.DropIndex(
                name: "IX_ExportDetails_TerminalId",
                table: "ExportDetails");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "ExportDetails");
        }
    }
}
