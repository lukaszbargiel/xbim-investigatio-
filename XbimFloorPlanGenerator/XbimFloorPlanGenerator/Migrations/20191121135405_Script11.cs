using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class Script11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductShape");

            migrationBuilder.CreateTable(
                name: "Opening",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WallId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    EntityLabel = table.Column<string>(nullable: true),
                    SerializedShapeGeometry = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opening", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opening_Wall_WallId",
                        column: x => x.WallId,
                        principalTable: "Wall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Window",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FloorId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    OverallHeight = table.Column<double>(nullable: false),
                    OverallWidth = table.Column<double>(nullable: false),
                    EntityLabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SerializedShapeGeometry = table.Column<string>(nullable: true),
                    IsExternal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Window", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Window_Floor_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Opening_WallId",
                table: "Opening",
                column: "WallId");

            migrationBuilder.CreateIndex(
                name: "IX_Window_FloorId",
                table: "Window",
                column: "FloorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opening");

            migrationBuilder.DropTable(
                name: "Window");

            migrationBuilder.CreateTable(
                name: "ProductShape",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoundingBoxSizeX = table.Column<double>(type: "float", nullable: false),
                    BoundingBoxSizeY = table.Column<double>(type: "float", nullable: false),
                    BoundingBoxX = table.Column<double>(type: "float", nullable: false),
                    BoundingBoxY = table.Column<double>(type: "float", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ShapeType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShape", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductShape_Wall_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Wall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductShape_ProductId",
                table: "ProductShape",
                column: "ProductId");
        }
    }
}
