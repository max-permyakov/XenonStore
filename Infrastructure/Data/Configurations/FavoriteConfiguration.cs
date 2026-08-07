using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.ToTable("Favorites");
            builder.HasKey(f => f.FavoriteId);

            builder.Property(f => f.UserId)
                .HasMaxLength(450);
            builder.Property(f => f.SessionId)
                .HasMaxLength(450);

            builder.HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(f => new { f.ProductId, f.UserId })
                .HasFilter("[UserId] IS NOT NULL")
                .IsUnique();

            builder.HasIndex(f => new { f.ProductId, f.SessionId })
                .HasFilter("[SessionId] IS NOT NULL")
                .IsUnique();
        }
    }
}
