using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedules.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartureDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Vessel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeparturePortCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DepartureCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ArrivalPortCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ArrivalCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CarrierCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VoyageNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Arrival = table.Column<DateOnly>(type: "date", nullable: false),
                    TransitTime = table.Column<long>(type: "bigint", nullable: false),
                    CutoffDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PortCutoffDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RateCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ContainerSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RateAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RateRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidityDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FreeTimeAtPOD = table.Column<int>(type: "int", nullable: false),
                    FreeTimeAtPOL = table.Column<int>(type: "int", nullable: false),
                    TransshipmentData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules");
        }
    }
}
