using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class script10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpaceCoordinates",
                table: "Space");

            migrationBuilder.AddColumn<string>(
                name: "SerializedShapeGeometry",
                table: "Wall",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerializedShapeGeometry",
                table: "Space",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedShapeGeometry",
                table: "Wall");

            migrationBuilder.DropColumn(
                name: "SerializedShapeGeometry",
                table: "Space");

            migrationBuilder.AddColumn<string>(
                name: "SpaceCoordinates",
                table: "Space",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
