using Xenon.Domain.Models;

namespace Xenon.Web.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
            = Enumerable.Empty<Product>();
        public PagingInfo PagingInfo { get; set; } = new();
        public string? CurrentCategory { get; set; }
        public string? SearchTerm { get; set; }
    }
}