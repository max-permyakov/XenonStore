using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Type { get; set; } = "Info";

        public bool IsRead { get; set; } = false;

        [MaxLength(200)]
        public string? Link { get; set; }
    }
}
