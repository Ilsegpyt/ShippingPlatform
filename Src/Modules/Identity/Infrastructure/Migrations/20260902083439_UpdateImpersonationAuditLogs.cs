using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImpersonationAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ImpersonationAuditLogs",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ImpersonationAuditLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TokenId",
                table: "ImpersonationAuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "ImpersonationAuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationAuditLogs_StartedAtUtc",
                table: "ImpersonationAuditLogs",
                column: "StartedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImpersonationAuditLogs_StartedAtUtc",
                table: "ImpersonationAuditLogs");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ImpersonationAuditLogs");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ImpersonationAuditLogs");

            migrationBuilder.DropColumn(
                name: "TokenId",
                table: "ImpersonationAuditLogs");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "ImpersonationAuditLogs");
        }
    }
}
