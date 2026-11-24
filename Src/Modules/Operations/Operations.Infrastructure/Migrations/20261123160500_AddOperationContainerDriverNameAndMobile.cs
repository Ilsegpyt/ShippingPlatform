using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Operations.Infrastructure.Persistence;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    [DbContext(typeof(OperationsDbContext))]
    [Migration("20261123160500_AddOperationContainerDriverNameAndMobile")]
    public partial class AddOperationContainerDriverNameAndMobile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "OperationContainers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverMobile",
                table: "OperationContainers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "OperationContainers");

            migrationBuilder.DropColumn(
                name: "DriverMobile",
                table: "OperationContainers");
        }
    }
}