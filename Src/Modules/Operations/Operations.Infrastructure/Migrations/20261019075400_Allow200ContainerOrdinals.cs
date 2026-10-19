using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Allow200ContainerOrdinals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OperationContainer_Ordinal",
                table: "OperationContainers");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OperationContainer_Ordinal",
                table: "OperationContainers",
                sql: "[Ordinal] >= 1 AND [Ordinal] <= 200");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OperationContainer_Ordinal",
                table: "OperationContainers");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OperationContainer_Ordinal",
                table: "OperationContainers",
                sql: "[Ordinal] >= 0 AND [Ordinal] < 200");
        }
    }
}
