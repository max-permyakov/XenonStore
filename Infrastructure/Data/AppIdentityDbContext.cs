using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<Product>();
            builder.Ignore<Category>();
            builder.Ignore<Supplier>();

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Address)
                    .HasMaxLength(200);

                entity.Property(u => u.City)
                    .HasMaxLength(100);

                entity.Property(u => u.Country)
                    .HasMaxLength(100);

                entity.Property(u => u.PostalCode)
                    .HasMaxLength(10);

                entity.Property(u => u.AvatarUrl)
                    .HasMaxLength(500);

                entity.Property(u => u.SupplierId)
                    .IsRequired(false);

                entity.Ignore(u => u.Supplier);

                entity.HasIndex(u => u.Email);
                entity.HasIndex(u => u.SupplierId);
            });
        }
    }
}
