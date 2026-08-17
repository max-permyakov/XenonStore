// Xenon.Domain/Interfaces/Services/IProductService.cs
using Xenon.Domain.Entities;
using Xenon.Domain.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, ProductFilter? filter = null);
    Task<Product?> GetProductAsync(long id);
    Task<int> GetTotalCountAsync(ProductFilter? filter = null);
}
