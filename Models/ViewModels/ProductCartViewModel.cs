using SportsStore.Models;
using SportsStore.Models.ViewModels;

public class ProductCartViewModel
{
    public Product? Product { get; set; } = null!;
    public int QuantityInCart { get; set; }  
}

public class ProductsListWithCartViewModel
{
    public IEnumerable<ProductCartViewModel> Products { get; set; }
        = Enumerable.Empty<ProductCartViewModel>();
    public PagingInfo PagingInfo { get; set; } = new();
    public string? CurrentCategory { get; set; }
}