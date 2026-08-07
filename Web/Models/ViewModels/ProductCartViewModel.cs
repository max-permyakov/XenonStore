
using Xenon.Domain.Models;
namespace Xenon.Web.Models.ViewModels
{
    
    public class ProductCartViewModel
    {
        public Product? Product { get; set; } = null!;
        public int QuantityInCart { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class ProductsListWithCartViewModel
    {
        public IEnumerable<ProductCartViewModel> Products { get; set; }
            = Enumerable.Empty<ProductCartViewModel>();
        public PagingInfo PagingInfo { get; set; } = new();
        public string? CurrentCategory { get; set; }
        public object? SearchTerm { get;  set; }
    }
    public class LoadMoreResult
    {
        public IEnumerable<ProductCartViewModel> Products { get; set; } = Enumerable.Empty<ProductCartViewModel>();
        public bool HasMore { get; set; }
        public int CurrentPage { get; set; }
    }
}