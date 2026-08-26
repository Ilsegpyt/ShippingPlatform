using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameGrantTypeToScopeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByIp",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ImpersonationAuditLogs");

            migrationBuilder.RenameColumn(
                name: "GrantType",
                table: "SubAccounts",
                newName: "ScopeType");

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationAuditLogs_TargetCustomerUserId",
                table: "ImpersonationAuditLogs",
                column: "TargetCustomerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImpersonationAuditLogs_TargetCustomerUserId",
                table: "ImpersonationAuditLogs");

            migrationBuilder.RenameColumn(
                name: "ScopeType",
                table: "SubAccounts",
                newName: "GrantType");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIp",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ImpersonationAuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
