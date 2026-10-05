using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoreOperationEnumsAsStrings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the old constraint before changing the column type.
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExportDetails_ClearanceType",
                table: "ExportDetails");

            // Change enum columns from int to string.
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Operations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OperationType",
                table: "Operations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ClearanceType",
                table: "ExportDetails",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Convert existing numeric enum values to their names.
            migrationBuilder.Sql("""
                UPDATE Operations
                SET OperationType =
                    CASE OperationType
                        WHEN '1' THEN 'Import'
                        WHEN '2' THEN 'Export'
                        ELSE OperationType
                    END;

                UPDATE Operations
                SET Status =
                    CASE Status
                        WHEN '1' THEN 'Open'
                        WHEN '2' THEN 'Delivered'
                        ELSE Status
                    END;

                UPDATE ExportDetails
                SET ClearanceType =
                    CASE ClearanceType
                        WHEN '1' THEN 'Bosla'
                        WHEN '2' THEN 'Cert'
                        ELSE ClearanceType
                    END;
                """);

            // Add the new string-based constraint.
            migrationBuilder.AddCheckConstraint(
                name: "CK_ExportDetails_ClearanceType",
                table: "ExportDetails",
                sql: "[ClearanceType] IN ('Bosla', 'Cert')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExportDetails_ClearanceType",
                table: "ExportDetails");

            // Convert enum names back to numeric values BEFORE
            // changing the columns back to int.
            migrationBuilder.Sql("""
                UPDATE Operations
                SET OperationType =
                    CASE OperationType
                        WHEN 'Import' THEN '1'
                        WHEN 'Export' THEN '2'
                        ELSE OperationType
                    END;

                UPDATE Operations
                SET Status =
                    CASE Status
                        WHEN 'Open' THEN '1'
                        WHEN 'Delivered' THEN '2'
                        ELSE Status
                    END;

                UPDATE ExportDetails
                SET ClearanceType =
                    CASE ClearanceType
                        WHEN 'Bosla' THEN '1'
                        WHEN 'Cert' THEN '2'
                        ELSE ClearanceType
                    END;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Operations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "OperationType",
                table: "Operations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "ClearanceType",
                table: "ExportDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExportDetails_ClearanceType",
                table: "ExportDetails",
                sql: "[ClearanceType] IN (1, 2)");
        }
    }
}