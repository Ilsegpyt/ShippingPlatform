using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Operations.Infrastructure.Persistence;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    [DbContext(typeof(OperationsDbContext))]
    [Migration("20261122123000_AddFclOperationDeliveredByName")]
    public partial class AddFclOperationDeliveredByName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveredByName",
                table: "Operations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            // Backfill existing delivered operations names from InternalUsers
            migrationBuilder.Sql(@"
                UPDATE o
                SET o.DeliveredByName = u.Name
                FROM Operations o
                INNER JOIN InternalUsers u ON o.DeliveredByUserId = CAST(u.UserId AS NVARCHAR(128))
                WHERE o.DeliveredByName IS NULL AND o.DeliveredByUserId IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredByName",
                table: "Operations");
        }
    }
}
