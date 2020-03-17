﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XbimFloorPlanGenerator.Data;

namespace XbimFloorPlanGenerator.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Building", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ElevationOfTerrain")
                        .HasColumnType("float");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SiteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("Building");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Floor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BuildingId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Elevation")
                        .HasColumnType("float");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("Floor");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.IfcFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FullPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("WasProcessed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("IfcFiles");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Opening", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerializedShapeGeometry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WallId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WallId");

                    b.ToTable("Opening");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProjectSet");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Site");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Space", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FloorId")
                        .HasColumnType("int");

                    b.Property<double>("GrossFloorArea")
                        .HasColumnType("float");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LongName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("NetFloorArea")
                        .HasColumnType("float");

                    b.Property<string>("SerializedBoundryData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerializedShapeGeometry")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.ToTable("Space");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Stair", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FloorId")
                        .HasColumnType("int");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerializedShapeGeometry")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.ToTable("Stair");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Wall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FloorId")
                        .HasColumnType("int");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsExternal")
                        .HasColumnType("bit");

                    b.Property<string>("SerializedShapeGeometry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("WallSideArea")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.ToTable("Wall");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Window", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FloorId")
                        .HasColumnType("int");

                    b.Property<string>("IfcId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IfcName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsExternal")
                        .HasColumnType("bit");

                    b.Property<double>("OverallHeight")
                        .HasColumnType("float");

                    b.Property<double>("OverallWidth")
                        .HasColumnType("float");

                    b.Property<string>("SerializedShapeGeometry")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.ToTable("Window");
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Building", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Site", "Site")
                        .WithMany("Buildings")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Floor", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Building", "Building")
                        .WithMany("Floors")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Opening", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Wall", "Wall")
                        .WithMany("Openings")
                        .HasForeignKey("WallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Site", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Project", "Project")
                        .WithMany("Sites")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Space", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Floor", "Floor")
                        .WithMany("Spaces")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Stair", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Floor", "Floor")
                        .WithMany("Stairs")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Wall", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Floor", "Floor")
                        .WithMany("Walls")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XbimFloorPlanGenerator.Data.Entities.Window", b =>
                {
                    b.HasOne("XbimFloorPlanGenerator.Data.Entities.Floor", "Floor")
                        .WithMany("Windows")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
