using Microsoft.EntityFrameworkCore;
using ProductCatalogService.Models;

namespace ProductCatalog.Data
{
    public class ProductCatalogDbContext : DbContext
    {
        public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
