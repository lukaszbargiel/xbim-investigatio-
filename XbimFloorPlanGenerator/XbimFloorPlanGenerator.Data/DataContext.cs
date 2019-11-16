using Microsoft.EntityFrameworkCore;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Project> ProjectSet { get; set; }

        public DbSet<IfcFile> IfcFiles { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>()
                .HasOne(p => p.Project)
                .WithMany(b => b.Sites);

            modelBuilder.Entity<Building>()
                .HasOne(p => p.Site)
                .WithMany(b => b.Buildings);

            modelBuilder.Entity<Floor>()
                .HasOne(p => p.Building)
                .WithMany(b => b.Floors);

            modelBuilder.Entity<Wall>()
                .HasOne(p => p.Floor)
                .WithMany(b => b.Walls);

            modelBuilder.Entity<Space>()
                .HasOne(p => p.Floor)
                .WithMany(b => b.Spaces);

            modelBuilder.Entity<ProductShape>()
    .HasOne(p => p.Wall)
    .WithMany(b => b.ProductShapes)
    .HasForeignKey(p => p.ProductId);
        }
    }
}
