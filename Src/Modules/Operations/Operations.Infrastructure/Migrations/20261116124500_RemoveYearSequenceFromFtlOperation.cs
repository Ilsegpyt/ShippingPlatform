using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveYearSequenceFromFtlOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "FtlOperations");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "FtlOperations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "FtlOperations",
                type: "int",
                nullable: false,
                defaultValue: 2026);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "FtlOperations",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }
    }
}
