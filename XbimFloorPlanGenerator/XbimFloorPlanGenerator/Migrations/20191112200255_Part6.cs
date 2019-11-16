using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class Part6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Space",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FloorId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    GrossFloorArea = table.Column<double>(nullable: false),
                    EntityLabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SpaceCoordinates = table.Column<string>(nullable: true),
                    NetFloorArea = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Space", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Space_Floor_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Space_FloorId",
                table: "Space",
                column: "FloorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Space");
        }
    }
}
