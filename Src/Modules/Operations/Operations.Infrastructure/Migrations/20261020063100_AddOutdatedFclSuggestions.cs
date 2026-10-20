using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutdatedFclSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions");

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_ImportContainerId_ExportContainerId",
                table: "FclSuggestions",
                columns: new[] { "ImportContainerId", "ExportContainerId" },
                unique: true,
                filter: "[ExportContainerId] IS NOT NULL AND [Status] = 'Pending'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions",
                sql: "[Status] IN ('Pending', 'Approved', 'Outdated')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FclSuggestions_ImportContainerId_ExportContainerId",
                table: "FclSuggestions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions",
                sql: "[Status] IN ('Pending', 'Approved')");
        }
    }
}
