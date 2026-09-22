namespace Xenon.Domain.Models
{
    public class RecentlyViewed
    {
        public long RecentlyViewedId { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public long ProductId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        public Product? Product { get; set; }
    }
}