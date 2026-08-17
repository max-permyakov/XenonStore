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

            builder.Property(o => o.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Building)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Apartment)
                .HasMaxLength(50);

            builder.Property(o => o.PostalCode)
                .HasMaxLength(10);

            builder.Property(o => o.Latitude);

            builder.Property(o => o.Longitude);

            builder.Property(o => o.DeliveryMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.PaymentId)
                .HasMaxLength(100);

            builder.Property(o => o.ShippingCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Comment)
                .HasMaxLength(1000);

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.Shipped)
                .IsRequired();

            builder.HasMany(o => o.Lines)
                .WithOne()
                .HasForeignKey("OrderID")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.Name);
            builder.HasIndex(o => o.Phone);
            builder.HasIndex(o => o.Email);
            builder.HasIndex(o => o.OrderDate);
            builder.HasIndex(o => o.DeliveryMethod);
            builder.HasIndex(o => o.PaymentMethod);
            builder.HasIndex(o => o.PaymentStatus);
            builder.HasIndex(o => o.Shipped);

            builder.Property(o => o.UserId)
                .HasMaxLength(450);

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(o => o.UserId);
        }
    }
}
