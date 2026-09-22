using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class RecentlyViewedConfiguration : IEntityTypeConfiguration<RecentlyViewed>
    {
        public void Configure(EntityTypeBuilder<RecentlyViewed> builder)
        {
            builder.ToTable("RecentlyViewed");
            builder.HasKey(r => r.RecentlyViewedId);

            builder.Property(r => r.UserId)
                .HasMaxLength(450);
            builder.Property(r => r.SessionId)
                .HasMaxLength(450);

            builder.HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.UserId, r.ViewedAt })
                .HasFilter("[UserId] IS NOT NULL");

            builder.HasIndex(r => new { r.SessionId, r.ViewedAt })
                .HasFilter("[SessionId] IS NOT NULL");

            builder.HasIndex(r => new { r.UserId, r.ProductId })
                .HasFilter("[UserId] IS NOT NULL")
                .IsUnique();

            builder.HasIndex(r => new { r.SessionId, r.ProductId })
                .HasFilter("[SessionId] IS NOT NULL")
                .IsUnique();
        }
    }
}