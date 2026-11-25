using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomsClearance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandCustomsContainerTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OceanImportDetails_ContainerType",
                schema: "customs",
                table: "OceanImportDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OceanExportDetails_ContainerType",
                schema: "customs",
                table: "OceanExportDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomsContainers_ContainerType",
                schema: "customs",
                table: "CustomsContainers");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerType",
                schema: "customs",
                table: "OceanImportDetails",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerType",
                schema: "customs",
                table: "OceanExportDetails",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerType",
                schema: "customs",
                table: "CustomsContainers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OceanImportDetails_ContainerType",
                schema: "customs",
                table: "OceanImportDetails",
                sql: "[ContainerType] IS NULL OR [ContainerType] IN ('DryStandard20', 'DryStandard40', 'DryHigh40', 'DryHigh45', 'ReeferStandard20', 'ReeferHigh40', 'OpenTop20', 'OpenTop40', 'OpenTopHigh40', 'FlatStandard40', 'FlatHigh40', 'Flat20', 'Tank20', 'Tank40', '20', '40', '40 HC', '45 HC')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OceanExportDetails_ContainerType",
                schema: "customs",
                table: "OceanExportDetails",
                sql: "[ContainerType] IS NULL OR [ContainerType] IN ('DryStandard20', 'DryStandard40', 'DryHigh40', 'DryHigh45', 'ReeferStandard20', 'ReeferHigh40', 'OpenTop20', 'OpenTop40', 'OpenTopHigh40', 'FlatStandard40', 'FlatHigh40', 'Flat20', 'Tank20', 'Tank40', '20', '40', '40 HC', '45 HC')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomsContainers_ContainerType",
                schema: "customs",
                table: "CustomsContainers",
                sql: "[ContainerType] IN ('DryStandard20', 'DryStandard40', 'DryHigh40', 'DryHigh45', 'ReeferStandard20', 'ReeferHigh40', 'OpenTop20', 'OpenTop40', 'OpenTopHigh40', 'FlatStandard40', 'FlatHigh40', 'Flat20', 'Tank20', 'Tank40', '20', '40', '40 HC', '45 HC')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OceanImportDetails_ContainerType",
                schema: "customs",
                table: "OceanImportDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OceanExportDetails_ContainerType",
                schema: "customs",
                table: "OceanExportDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomsContainers_ContainerType",
                schema: "customs",
                table: "CustomsContainers");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerType",
                schema: "customs",
                table: "OceanImportDetails",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerType",
                schema: "customs",
                table: "OceanExportDetails",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerType",
                schema: "customs",
                table: "CustomsContainers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OceanImportDetails_ContainerType",
                schema: "customs",
                table: "OceanImportDetails",
                sql: "[ContainerType] IS NULL OR [ContainerType] IN ('20', '40', '40 HC', '45 HC')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OceanExportDetails_ContainerType",
                schema: "customs",
                table: "OceanExportDetails",
                sql: "[ContainerType] IS NULL OR [ContainerType] IN ('20', '40', '40 HC', '45 HC')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomsContainers_ContainerType",
                schema: "customs",
                table: "CustomsContainers",
                sql: "[ContainerType] IN ('20', '40', '40 HC', '45 HC')");
        }
    }
}
