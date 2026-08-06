namespace Xenon.Domain.Models
{
    public enum ProductSortBy
    {
        Popularity,
        Price,
        Rating
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class ProductFilter
    {
        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinRating { get; set; }
        public string? Supplier { get; set; }
        public ProductSortBy SortBy { get; set; } = ProductSortBy.Popularity;
        public SortDirection Direction { get; set; } = SortDirection.Descending;
    }
}
