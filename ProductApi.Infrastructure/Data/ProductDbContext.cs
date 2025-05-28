using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Enities;


namespace ProductApi.Infrastructure.Data
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options):DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 4); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
