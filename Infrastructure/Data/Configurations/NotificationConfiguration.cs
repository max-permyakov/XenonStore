using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(n => n.Link)
                .HasMaxLength(200);

            builder.HasIndex(n => n.CreatedAt);
            builder.HasIndex(n => n.IsRead);
        }
    }
}
