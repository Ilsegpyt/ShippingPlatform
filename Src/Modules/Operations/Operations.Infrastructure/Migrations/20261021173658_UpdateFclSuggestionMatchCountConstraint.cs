using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFclSuggestionMatchCountConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FclSuggestions_MatchCount",
                table: "FclSuggestions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FclSuggestions_MatchCount",
                table: "FclSuggestions",
                sql: "[MatchCount] = 6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FclSuggestions_MatchCount",
                table: "FclSuggestions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FclSuggestions_MatchCount",
                table: "FclSuggestions",
                sql: "[MatchCount] >= 5 AND [MatchCount] <= 6");
        }
    }
}
