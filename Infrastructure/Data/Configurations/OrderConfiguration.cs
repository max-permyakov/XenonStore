using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderID);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Line1)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Line2)
                .HasMaxLength(200);

            builder.Property(o => o.Line3)
                .HasMaxLength(200);

            builder.Property(o => o.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.State)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Zip)
                .HasMaxLength(20);

            builder.Property(o => o.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.GiftWrap)
                .IsRequired();

            builder.Property(o => o.Shipped)
                .IsRequired();

            builder.HasMany(o => o.Lines)
                .WithOne()
                .HasForeignKey("OrderID")
                .OnDelete(DeleteBehavior.Cascade);

          
            builder.HasIndex(o => o.Name);
            builder.HasIndex(o => o.Shipped);
        }
    }
}