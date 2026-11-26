using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomsClearance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomsFinanceRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomsFinanceRequests",
                schema: "customs",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeferredUntilDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsFinanceRequests", x => x.ShipmentId);
                    table.CheckConstraint("CK_CustomsFinanceRequests_DeferredUntilDate", "[Status] <> 'Deferred' OR [DeferredUntilDate] IS NOT NULL");
                    table.CheckConstraint("CK_CustomsFinanceRequests_Status", "[Status] IN ('Pending', 'Approved', 'Deferred', 'OnHold')");
                    table.ForeignKey(
                        name: "FK_CustomsFinanceRequests_CustomsShipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "customs",
                        principalTable: "CustomsShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomsFinanceRequests_Status_DeferredUntilDate",
                schema: "customs",
                table: "CustomsFinanceRequests",
                columns: new[] { "Status", "DeferredUntilDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomsFinanceRequests",
                schema: "customs");
        }
    }
}
