namespace Xenon.Application.DTOs
{
    public class UpdateProductDto
    {
        public long ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public long SupplierId { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public decimal? Discount { get; set; }
    }
}