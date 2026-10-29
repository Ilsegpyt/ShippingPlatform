using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDestinationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_Name",
                table: "Destinations",
                column: "Name",
                unique: true);

            // Seed initial common destinations so they are available right away
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Destinations WHERE Name = '10TH OF RAMADAN')
                BEGIN
                    INSERT INTO Destinations (Id, Name, IsActive) VALUES
                    (NEWID(), '10TH OF RAMADAN', 1),
                    (NEWID(), '6TH OF OCTOBER', 1),
                    (NEWID(), 'ABU RAWASH', 1),
                    (NEWID(), 'ALEXANDRIA', 1),
                    (NEWID(), 'BORG EL ARAB', 1),
                    (NEWID(), 'EL SADAT', 1),
                    (NEWID(), 'OCTOBER', 1),
                    (NEWID(), 'SOKHNA', 1),
                    (NEWID(), 'WADI EL NATRUN', 1);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destinations");
        }
    }
}
