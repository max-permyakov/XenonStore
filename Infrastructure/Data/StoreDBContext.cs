using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data.Configurations;

namespace Xenon.Infrastructure.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<UserCart> UserCarts => Set<UserCart>();
        public DbSet<LogEntry> LogEntries => Set<LogEntry>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
        }
    }
}
