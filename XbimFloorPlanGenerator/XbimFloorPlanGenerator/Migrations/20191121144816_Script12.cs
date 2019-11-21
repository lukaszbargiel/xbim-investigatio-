using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class Script12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LongName",
                table: "Space",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LongName",
                table: "Space");
        }
    }
}
