using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationIdsToCustomerIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "SubAccounts",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_SubAccounts_OrganizationId",
                table: "SubAccounts",
                newName: "IX_SubAccounts_CustomerId");

            migrationBuilder.RenameColumn(
                name: "ImpersonatedOrganizationId",
                table: "RefreshTokens",
                newName: "ImpersonatedCustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_ImpersonatedOrganizationId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_ImpersonatedCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "SubAccounts",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_SubAccounts_CustomerId",
                table: "SubAccounts",
                newName: "IX_SubAccounts_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "ImpersonatedCustomerId",
                table: "RefreshTokens",
                newName: "ImpersonatedOrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_ImpersonatedCustomerId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_ImpersonatedOrganizationId");
        }
    }
}
