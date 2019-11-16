using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class Part5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductShape_Wall_WallId",
                table: "ProductShape");

            migrationBuilder.DropIndex(
                name: "IX_ProductShape_WallId",
                table: "ProductShape");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Wall");

            migrationBuilder.DropColumn(
                name: "WallId",
                table: "ProductShape");

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "Wall",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "WallSideArea",
                table: "Wall",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Elevation",
                table: "Floor",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductShape_ProductId",
                table: "ProductShape",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductShape_Wall_ProductId",
                table: "ProductShape",
                column: "ProductId",
                principalTable: "Wall",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductShape_Wall_ProductId",
                table: "ProductShape");

            migrationBuilder.DropIndex(
                name: "IX_ProductShape_ProductId",
                table: "ProductShape");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "Wall");

            migrationBuilder.DropColumn(
                name: "WallSideArea",
                table: "Wall");

            migrationBuilder.DropColumn(
                name: "Elevation",
                table: "Floor");

            migrationBuilder.AddColumn<double>(
                name: "Area",
                table: "Wall",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WallId",
                table: "ProductShape",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductShape_WallId",
                table: "ProductShape",
                column: "WallId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductShape_Wall_WallId",
                table: "ProductShape",
                column: "WallId",
                principalTable: "Wall",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
