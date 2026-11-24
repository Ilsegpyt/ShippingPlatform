using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transporters.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PortScopedClientExceptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransporterClientExceptions_Transporters_TransporterId",
                schema: "transporters",
                table: "TransporterClientExceptions");

            migrationBuilder.DropColumn(
                name: "TahweelaPrice",
                schema: "transporters",
                table: "TransporterPortCoverages");

            migrationBuilder.DropColumn(
                name: "OriginPort",
                schema: "transporters",
                table: "TransporterClientExceptions");

            migrationBuilder.AddColumn<Guid>(
                name: "PortCoverageId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransporterClientExceptions_PortCoverageId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                column: "PortCoverageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransporterClientExceptions_TransporterPortCoverages_PortCoverageId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                column: "PortCoverageId",
                principalSchema: "transporters",
                principalTable: "TransporterPortCoverages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransporterClientExceptions_Transporters_TransporterId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                column: "TransporterId",
                principalSchema: "transporters",
                principalTable: "Transporters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransporterClientExceptions_TransporterPortCoverages_PortCoverageId",
                schema: "transporters",
                table: "TransporterClientExceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TransporterClientExceptions_Transporters_TransporterId",
                schema: "transporters",
                table: "TransporterClientExceptions");

            migrationBuilder.DropIndex(
                name: "IX_TransporterClientExceptions_PortCoverageId",
                schema: "transporters",
                table: "TransporterClientExceptions");

            migrationBuilder.DropColumn(
                name: "PortCoverageId",
                schema: "transporters",
                table: "TransporterClientExceptions");

            migrationBuilder.AddColumn<decimal>(
                name: "TahweelaPrice",
                schema: "transporters",
                table: "TransporterPortCoverages",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginPort",
                schema: "transporters",
                table: "TransporterClientExceptions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TransporterClientExceptions_Transporters_TransporterId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                column: "TransporterId",
                principalSchema: "transporters",
                principalTable: "Transporters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
