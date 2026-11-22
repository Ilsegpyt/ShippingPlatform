using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Operations.Infrastructure.Persistence;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    [DbContext(typeof(OperationsDbContext))]
    [Migration("20261118131500_AddFtlOperationDeliveryAudit")]
    public partial class AddFtlOperationDeliveryAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAtUtc",
                table: "FtlOperations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveredByUserId",
                table: "FtlOperations",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveredByName",
                table: "FtlOperations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FtlOperations_Status_DeliveredAtUtc",
                table: "FtlOperations",
                columns: new[] { "Status", "DeliveredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FtlOperations_Status_DeliveredAtUtc",
                table: "FtlOperations");

            migrationBuilder.DropColumn(
                name: "DeliveredAtUtc",
                table: "FtlOperations");

            migrationBuilder.DropColumn(
                name: "DeliveredByUserId",
                table: "FtlOperations");

            migrationBuilder.DropColumn(
                name: "DeliveredByName",
                table: "FtlOperations");
        }
    }
}
