using Microsoft.EntityFrameworkCore;
using Tech_Desk.Models;
namespace Tech_Desk.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
             : base(options) { }

        public DbSet<Device> Devices => Set<Device>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique index on AssetTag
            modelBuilder.Entity<Device>()
                        .HasIndex(d => d.AssetTag)
                        .IsUnique();

            // Seed the five devices
            modelBuilder.Entity<Device>().HasData(
                new Device { Id = 1, AssetTag = "ECC-0042", Name = "Dell Latitude 5440", Category = "Laptop", PurchaseYear = 2023, IsOnLoan = true },
                new Device { Id = 2, AssetTag = "ECC-0007", Name = "HP ProBook 450", Category = "Laptop", PurchaseYear = 2019, IsOnLoan = false },
                new Device { Id = 3, AssetTag = "ECC-0115", Name = "iPad 9th generation", Category = "Tablet", PurchaseYear = 2022, IsOnLoan = true },
                new Device { Id = 4, AssetTag = "ECC-0090", Name = "Samsung Galaxy Tab A8", Category = "Tablet", PurchaseYear = 2021, IsOnLoan = false },
                new Device { Id = 5, AssetTag = "ECC-0023", Name = "Lenovo ThinkPad E14", Category = "Laptop", PurchaseYear = 2017, IsOnLoan = false }
            );
        }
    }
}