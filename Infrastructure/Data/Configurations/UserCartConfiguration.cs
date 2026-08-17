using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class UserCartConfiguration : IEntityTypeConfiguration<UserCart>
    {
        public void Configure(EntityTypeBuilder<UserCart> builder)
        {
            builder.HasKey(uc => uc.UserId);

            builder.HasOne(uc => uc.User)
                .WithOne()
                .HasForeignKey<UserCart>(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<CartLine>()
                .WithOne()
                .HasForeignKey("UserCartId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
