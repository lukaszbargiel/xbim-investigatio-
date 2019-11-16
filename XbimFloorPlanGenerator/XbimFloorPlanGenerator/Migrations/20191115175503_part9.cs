using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class part9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "BoundingBoxY",
                table: "ProductShape",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");

            migrationBuilder.AlterColumn<double>(
                name: "BoundingBoxX",
                table: "ProductShape",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");

            migrationBuilder.AlterColumn<double>(
                name: "BoundingBoxSizeY",
                table: "ProductShape",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");

            migrationBuilder.AlterColumn<double>(
                name: "BoundingBoxSizeX",
                table: "ProductShape",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxY",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxX",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxSizeY",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxSizeX",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
