using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFclApprovalsDirectImportContainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add ImportContainerId as nullable initially to allow backfilling data for existing approvals
            migrationBuilder.AddColumn<Guid>(
                name: "ImportContainerId",
                table: "FclApprovals",
                type: "uniqueidentifier",
                nullable: true);

            // 2. Data migration: populate ImportContainerId from linked FclSuggestions for existing approvals
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.ImportContainerId = s.ImportContainerId
                FROM FclApprovals a
                INNER JOIN FclSuggestions s ON a.SuggestionId = s.Id
                WHERE a.ImportContainerId IS NULL;
            ");

            // 3. In case any legacy approvals could not be matched, delete them
            migrationBuilder.Sql(@"
                DELETE FROM FclApprovals
                WHERE ImportContainerId IS NULL;
            ");

            // 4. Alter ImportContainerId to NOT NULL
            migrationBuilder.AlterColumn<Guid>(
                name: "ImportContainerId",
                table: "FclApprovals",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // 5. Add unique index and foreign key for ImportContainerId
            migrationBuilder.CreateIndex(
                name: "UX_FclApprovals_ImportContainerId",
                table: "FclApprovals",
                column: "ImportContainerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FclApprovals_OperationContainers_ImportContainerId",
                table: "FclApprovals",
                column: "ImportContainerId",
                principalTable: "OperationContainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // 6. Drop foreign key and index on SuggestionId, then drop SuggestionId column
            migrationBuilder.DropForeignKey(
                name: "FK_FclApprovals_FclSuggestions_SuggestionId",
                table: "FclApprovals");

            migrationBuilder.DropIndex(
                name: "IX_FclApprovals_SuggestionId",
                table: "FclApprovals");

            migrationBuilder.DropColumn(
                name: "SuggestionId",
                table: "FclApprovals");

            // 7. Make ExportOperationId nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "ExportOperationId",
                table: "FclApprovals",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 8. Drop FclSuggestions table entirely
            migrationBuilder.DropTable(
                name: "FclSuggestions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FclSuggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ClearanceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContainerNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    ContainerSizeMatch = table.Column<bool>(type: "bit", nullable: false),
                    ContainerType = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CutOffDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DestinationMatch = table.Column<bool>(type: "bit", nullable: false),
                    ExportContainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExportOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FreeTimeMatch = table.Column<bool>(type: "bit", nullable: false),
                    ImportContainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchCount = table.Column<int>(type: "int", nullable: false),
                    OffloadingLoadingTimeMatch = table.Column<bool>(type: "bit", nullable: false),
                    POLId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    POWId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PodPolMatch = table.Column<bool>(type: "bit", nullable: false),
                    RequiredLoadingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequiredLoadingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ShippingLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingLineMatch = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FclSuggestions", x => x.Id);
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
                });

            migrationBuilder.AddColumn<Guid>(
                name: "SuggestionId",
                table: "FclApprovals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FclApprovals_SuggestionId",
                table: "FclApprovals",
                column: "SuggestionId",
                unique: true,
                filter: "[SuggestionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FclApprovals_FclSuggestions_SuggestionId",
                table: "FclApprovals",
                column: "SuggestionId",
                principalTable: "FclSuggestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropForeignKey(
                name: "FK_FclApprovals_OperationContainers_ImportContainerId",
                table: "FclApprovals");

            migrationBuilder.DropIndex(
                name: "UX_FclApprovals_ImportContainerId",
                table: "FclApprovals");

            migrationBuilder.DropColumn(
                name: "ImportContainerId",
                table: "FclApprovals");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExportOperationId",
                table: "FclApprovals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
