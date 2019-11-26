using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class scrip12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerializedBoundryData",
                table: "Space",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedBoundryData",
                table: "Space");
        }
    }
}
