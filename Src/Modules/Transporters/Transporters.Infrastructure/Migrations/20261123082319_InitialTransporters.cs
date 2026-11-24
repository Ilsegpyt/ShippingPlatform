using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transporters.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialTransporters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "transporters");

            migrationBuilder.CreateTable(
                name: "Transporters",
                schema: "transporters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transporters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransporterClientExceptions",
                schema: "transporters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginPort = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EquipmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AgreedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValidFromUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidToUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransporterClientExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransporterClientExceptions_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalSchema: "transporters",
                        principalTable: "Transporters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransporterPortCoverages",
                schema: "transporters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FreeHours = table.Column<int>(type: "int", nullable: false),
                    DelayRatePerHour = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TahweelaPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OfficialReceiptNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GeneralNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransporterPortCoverages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransporterPortCoverages_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalSchema: "transporters",
                        principalTable: "Transporters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransporterLaneRates",
                schema: "transporters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortCoverageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EquipmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransporterLaneRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransporterLaneRates_TransporterPortCoverages_PortCoverageId",
                        column: x => x.PortCoverageId,
                        principalSchema: "transporters",
                        principalTable: "TransporterPortCoverages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransporterClientExceptions_CustomerId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterClientExceptions_TransporterId",
                schema: "transporters",
                table: "TransporterClientExceptions",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterLaneRates_PortCoverageId",
                schema: "transporters",
                table: "TransporterLaneRates",
                column: "PortCoverageId");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterPortCoverages_TransporterId_PortName",
                schema: "transporters",
                table: "TransporterPortCoverages",
                columns: new[] { "TransporterId", "PortName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransporterClientExceptions",
                schema: "transporters");

            migrationBuilder.DropTable(
                name: "TransporterLaneRates",
                schema: "transporters");

            migrationBuilder.DropTable(
                name: "TransporterPortCoverages",
                schema: "transporters");

            migrationBuilder.DropTable(
                name: "Transporters",
                schema: "transporters");
        }
    }
}
