// Xenon.Domain/Interfaces/Services/IProductService.cs
using Xenon.Domain.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string? category = null);
    Task<Product?> GetProductAsync(long id);  // может вернуть null
    Task<int> GetTotalCountAsync(string? category = null);
}