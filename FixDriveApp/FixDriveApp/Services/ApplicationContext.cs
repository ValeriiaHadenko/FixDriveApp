using Microsoft.EntityFrameworkCore;
using FixDriveApp.Models;

namespace FixDriveApp.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=/Users/leroro/Desktop/Nure/Sqlite/Database/product.db");
        }
    }
}