using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImpersonationFieldsToRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenId",
                table: "ImpersonationAuditLogs");

            migrationBuilder.AddColumn<Guid>(
                name: "ImpersonatedOrganizationId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenType",
                table: "RefreshTokens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ImpersonatedOrganizationId",
                table: "RefreshTokens",
                column: "ImpersonatedOrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ImpersonatedOrganizationId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ImpersonatedOrganizationId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "TokenType",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "TokenId",
                table: "ImpersonationAuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
