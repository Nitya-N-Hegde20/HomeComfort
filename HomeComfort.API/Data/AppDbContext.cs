using HomeComfort.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeComfort.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Rating)
                .HasPrecision(3, 2);
            base.OnModelCreating(modelBuilder);

            // Seed categories first
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Furniture",
                    Description = "Home furniture collection",
                    Image = "https://via.placeholder.com/300",
                    CreatedAt = new DateTime(2026, 7, 1)
                },
                new Category
                {
                    Id = 2,
                    Name = "Kitchen",
                    Description = "Kitchen appliances and tools",
                    Image = "https://via.placeholder.com/300",
                    CreatedAt = new DateTime(2026, 7, 1)
                },
                new Category
                {
                    Id = 3,
                    Name = "Bedding",
                    Description = "Comfortable bedding and pillows",
                    Image = "https://via.placeholder.com/300",
                    CreatedAt = new DateTime(2026, 7, 1)
                }
            );
        }
    }
}
