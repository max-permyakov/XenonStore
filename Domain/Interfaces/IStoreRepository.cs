// Xenon.Domain/Interfaces/IStoreRepository.cs
using Xenon.Domain.Entities;
using Xenon.Domain.Models;

public interface IStoreRepository
{
    IQueryable<Product> Products { get; }

    Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, ProductFilter? filter = null);
    Task<Product?> GetProductAsync(long id);
    Task<int> GetTotalCountAsync(ProductFilter? filter = null);
    void SaveProduct(Product p);
    void CreateProduct(Product p);
    void DeleteProduct(Product p);
}