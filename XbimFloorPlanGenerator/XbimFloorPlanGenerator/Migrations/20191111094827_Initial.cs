using Microsoft.EntityFrameworkCore.Migrations;

namespace XbimFloorPlanGenerator.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    IfcFileName = table.Column<string>(nullable: true),
                    EntityLabel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    EntityLabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Site_ProjectSet_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Building",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    ElevationOfTerrain = table.Column<double>(nullable: false),
                    EntityLabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Building_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Floor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    EntityLabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Floor_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wall",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FloorId = table.Column<int>(nullable: false),
                    IfcId = table.Column<string>(nullable: true),
                    IfcName = table.Column<string>(nullable: true),
                    Area = table.Column<double>(nullable: false),
                    EntityLabel = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wall_Floor_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductShape",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    ShapeType = table.Column<int>(nullable: false),
                    BoundingBoxX = table.Column<double>(nullable: false),
                    BoundingBoxY = table.Column<double>(nullable: false),
                    BoundingBoxSizeX = table.Column<double>(nullable: false),
                    BoundingBoxSizeY = table.Column<double>(nullable: false),
                    WallId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShape", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductShape_Wall_WallId",
                        column: x => x.WallId,
                        principalTable: "Wall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Building_SiteId",
                table: "Building",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_BuildingId",
                table: "Floor",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductShape_WallId",
                table: "ProductShape",
                column: "WallId");

            migrationBuilder.CreateIndex(
                name: "IX_Site_ProjectId",
                table: "Site",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Wall_FloorId",
                table: "Wall",
                column: "FloorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductShape");

            migrationBuilder.DropTable(
                name: "Wall");

            migrationBuilder.DropTable(
                name: "Floor");

            migrationBuilder.DropTable(
                name: "Building");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "ProjectSet");
        }
    }
}
