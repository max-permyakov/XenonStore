using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class LogEntry
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string Level { get; set; } = "Info";

        [MaxLength(200)]
        public string Source { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? Exception { get; set; }

        [MaxLength(100)]
        public string? UserId { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(50)]
        public string? EntityType { get; set; }

        public long? EntityId { get; set; }

        [MaxLength(2000)]
        public string? Metadata { get; set; }

        public bool IsRead { get; set; } = false;
        public bool IsAlert { get; set; } = false;
    }
}
