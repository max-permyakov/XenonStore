namespace Xenon.Domain.Models
{
    public class Favorite
    {
        public long FavoriteId { get; set; }
        public long ProductId { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Product? Product { get; set; }
    }
}
