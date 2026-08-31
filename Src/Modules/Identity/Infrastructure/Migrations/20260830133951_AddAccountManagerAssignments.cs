using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountManagerAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountManagerAssignments_AccountManagerUserId_CustomerId",
                table: "AccountManagerAssignments");

            migrationBuilder.RenameColumn(
                name: "AccountManagerUserId",
                table: "AccountManagerAssignments",
                newName: "AccountManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountManagerAssignments_AccountManagerId",
                table: "AccountManagerAssignments",
                column: "AccountManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountManagerAssignments_CustomerId",
                table: "AccountManagerAssignments",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountManagerAssignments_AccountManagerId",
                table: "AccountManagerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AccountManagerAssignments_CustomerId",
                table: "AccountManagerAssignments");

            migrationBuilder.RenameColumn(
                name: "AccountManagerId",
                table: "AccountManagerAssignments",
                newName: "AccountManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountManagerAssignments_AccountManagerUserId_CustomerId",
                table: "AccountManagerAssignments",
                columns: new[] { "AccountManagerUserId", "CustomerId" },
                unique: true);
        }
    }
}
