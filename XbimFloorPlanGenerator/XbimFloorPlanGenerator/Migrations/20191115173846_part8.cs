using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class part8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxY",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxX",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxSizeY",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxSizeX",
                table: "ProductShape",
                type: "decimal(22,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxY",
                table: "ProductShape",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxX",
                table: "ProductShape",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxSizeY",
                table: "ProductShape",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BoundingBoxSizeX",
                table: "ProductShape",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,10)");
        }
    }
}
