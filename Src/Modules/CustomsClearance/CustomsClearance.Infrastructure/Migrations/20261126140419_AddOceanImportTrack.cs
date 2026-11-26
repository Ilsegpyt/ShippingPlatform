using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomsClearance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOceanImportTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Track",
                schema: "customs",
                table: "OceanImportDetails",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OceanImportDetails_Track",
                schema: "customs",
                table: "OceanImportDetails",
                sql: "[Track] IS NULL OR [Track] IN ('Blue', 'Yellow', 'Green', 'Red', 'GreenImports')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OceanImportDetails_Track",
                schema: "customs",
                table: "OceanImportDetails");

            migrationBuilder.DropColumn(
                name: "Track",
                schema: "customs",
                table: "OceanImportDetails");
        }
    }
}
