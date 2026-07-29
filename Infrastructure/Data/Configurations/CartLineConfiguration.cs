using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class CartLineConfiguration : IEntityTypeConfiguration<CartLine>
    {
        public void Configure(EntityTypeBuilder<CartLine> builder)
        {
            builder.HasKey(cl => cl.CartLineID);

            builder.Property(cl => cl.Quantity)
                .IsRequired();

            // Связь с Product
            builder.HasOne(cl => cl.Product)
                .WithMany()
                .HasForeignKey("ProductID")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}