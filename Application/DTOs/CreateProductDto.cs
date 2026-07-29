namespace Xenon.Application.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public long SupplierId { get; set; }
        public string? ImageUrl { get; set; }
    }
}