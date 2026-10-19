using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateContainerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "FreeTimeTill",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "ExportDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "ImportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FreeTimeTill",
                table: "ImportDetails",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "ExportDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
