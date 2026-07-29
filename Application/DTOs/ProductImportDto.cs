// Xenon.Application/DTOs/ProductImportDto.cs
using CsvHelper.Configuration.Attributes;

namespace Xenon.Application.DTOs
{
    public class ProductImportDto
    {
        [Name("Sub Category")]
        public string SubCategory { get; set; } = string.Empty;

        [Name("Price")]
        public string PriceString { get; set; } = string.Empty;

        [Name("Discount")]
        public string DiscountString { get; set; } = string.Empty;

        [Name("Rating")]
        public string RatingString { get; set; } = string.Empty;

        [Name("Title")]
        public string Title { get; set; } = string.Empty;

        [Name("Currency")]
        public string CurrencyString { get; set; } = string.Empty;

        [Name("Feature")]
        public string Feature { get; set; } = string.Empty;
    }
}