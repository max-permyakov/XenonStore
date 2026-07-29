// Xenon.Domain/Interfaces/IStoreRepository.cs
using Xenon.Domain.Models;

public interface IStoreRepository
{
    IQueryable<Product> Products { get; }

    Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string? category = null, string? searchTerm = null);
    Task<Product?> GetProductAsync(long id);
    Task<int> GetTotalCountAsync(string? category = null, string? searchTerm = null);
    void SaveProduct(Product p);
    void CreateProduct(Product p);
    void DeleteProduct(Product p);
}