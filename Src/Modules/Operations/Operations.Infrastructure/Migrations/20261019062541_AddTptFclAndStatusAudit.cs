using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTptFclAndStatusAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Operations",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "form");

            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeforeStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AfterStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.Id);
                    table.CheckConstraint("CK_audit_log_StatusChanged", "[BeforeStatus] <> [AfterStatus]");
                    table.ForeignKey(
                        name: "FK_audit_log_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FclSuggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportContainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClearanceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BookingNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ShippingLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    POLId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    POWId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CutOffDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ContainerNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    ContainerType = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequiredLoadingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequiredLoadingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    FreeTimeMatch = table.Column<bool>(type: "bit", nullable: false),
                    ShippingLineMatch = table.Column<bool>(type: "bit", nullable: false),
                    PodPolMatch = table.Column<bool>(type: "bit", nullable: false),
                    OffloadingLoadingTimeMatch = table.Column<bool>(type: "bit", nullable: false),
                    ContainerSizeMatch = table.Column<bool>(type: "bit", nullable: false),
                    DestinationMatch = table.Column<bool>(type: "bit", nullable: false),
                    MatchCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FclSuggestions", x => x.Id);
                    table.CheckConstraint("CK_FclSuggestions_ClearanceType", "[ClearanceType] IN ('Bosla', 'Cert')");
                    table.CheckConstraint("CK_FclSuggestions_MatchCount", "[MatchCount] >= 5 AND [MatchCount] <= 6");
                    table.CheckConstraint("CK_FclSuggestions_Status", "[Status] IN ('Pending', 'Approved')");
                    table.ForeignKey(
                        name: "FK_FclSuggestions_OperationContainers_ImportContainerId",
                        column: x => x.ImportContainerId,
                        principalTable: "OperationContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FclSuggestions_Operations_ImportOperationId",
                        column: x => x.ImportOperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FclSuggestions_Ports_POLId",
                        column: x => x.POLId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FclSuggestions_Ports_POWId",
                        column: x => x.POWId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FclSuggestions_ShippingLines_ShippingLineId",
                        column: x => x.ShippingLineId,
                        principalTable: "ShippingLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FclApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuggestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedExportOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalKey = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FclApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FclApprovals_FclSuggestions_SuggestionId",
                        column: x => x.SuggestionId,
                        principalTable: "FclSuggestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FclApprovals_Operations_CreatedExportOperationId",
                        column: x => x.CreatedExportOperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Operations_Source",
                table: "Operations",
                sql: "[Source] IN ('form', 'import', 'fcl')");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_OperationId_AtUtc",
                table: "audit_log",
                columns: new[] { "OperationId", "AtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_FclApprovals_CreatedExportOperationId",
                table: "FclApprovals",
                column: "CreatedExportOperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FclApprovals_SuggestionId",
                table: "FclApprovals",
                column: "SuggestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_FclApprovals_ApprovalKey",
                table: "FclApprovals",
                column: "ApprovalKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_ImportContainerId",
                table: "FclSuggestions",
                column: "ImportContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_ImportOperationId",
                table: "FclSuggestions",
                column: "ImportOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_POLId",
                table: "FclSuggestions",
                column: "POLId");

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_POWId",
                table: "FclSuggestions",
                column: "POWId");

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_ShippingLineId",
                table: "FclSuggestions",
                column: "ShippingLineId");

            migrationBuilder.CreateIndex(
                name: "IX_FclSuggestions_Status",
                table: "FclSuggestions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "FclApprovals");

            migrationBuilder.DropTable(
                name: "FclSuggestions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Operations_Source",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Operations");
        }
    }
}
