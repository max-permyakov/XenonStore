namespace Xenon.Application.DTOs
{
    public class ProductDto
    {
        public long ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public long SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public decimal? Discount { get; set; }
    }
}