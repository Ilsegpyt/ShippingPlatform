using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesForShipmentAndBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Operations_ShipmentNumber",
                table: "Operations",
                column: "ShipmentNumber",
                unique: true,
                filter: "[IsDeleted] = 0 AND [ShipmentNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_ExportDetails_BookingNumber",
                table: "ExportDetails",
                column: "BookingNumber",
                unique: true,
                filter: "[BookingNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Operations_ShipmentNumber",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "UX_ExportDetails_BookingNumber",
                table: "ExportDetails");
        }
    }
}
