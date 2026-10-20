using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RremoveOutdatedFclSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions",
                sql: "[Status] IN ('Pending', 'Approved')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FclSuggestions_Status",
                table: "FclSuggestions",
                sql: "[Status] IN ('Pending', 'Approved', 'Outdated')");
        }
    }
}
