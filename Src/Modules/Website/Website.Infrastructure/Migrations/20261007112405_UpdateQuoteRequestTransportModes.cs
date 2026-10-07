using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Website.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuoteRequestTransportModes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransportMode",
                table: "QuoteRequests");

            migrationBuilder.AddColumn<string>(
                name: "TransportModes",
                table: "QuoteRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransportModes",
                table: "QuoteRequests");

            migrationBuilder.AddColumn<string>(
                name: "TransportMode",
                table: "QuoteRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
