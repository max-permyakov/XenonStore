using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Data.Configurations
{
    public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.HasKey(le => le.Id);

            builder.Property(le => le.Timestamp)
                .IsRequired();

            builder.Property(le => le.Level)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(le => le.Source)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(le => le.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(le => le.Exception)
                .HasMaxLength(4000);

            builder.Property(le => le.UserId)
                .HasMaxLength(100);

            builder.Property(le => le.UserName)
                .HasMaxLength(100);

            builder.Property(le => le.IpAddress)
                .HasMaxLength(45);

            builder.Property(le => le.EntityType)
                .HasMaxLength(50);

            builder.Property(le => le.Metadata)
                .HasMaxLength(2000);

            builder.HasIndex(le => le.Timestamp);
            builder.HasIndex(le => le.Level);
            builder.HasIndex(le => le.Source);
            builder.HasIndex(le => le.UserId);
            builder.HasIndex(le => le.IsAlert);
        }
    }
}
