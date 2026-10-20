using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeImportExportDuplicateKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuplicateCheckKey",
                table: "Operations",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_Operations_OperationType_DuplicateCheckKey",
                table: "Operations",
                columns: new[] { "OperationType", "DuplicateCheckKey" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [DuplicateCheckKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Operations_OperationType_DuplicateCheckKey",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "DuplicateCheckKey",
                table: "Operations");
        }
    }
}
