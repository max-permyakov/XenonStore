// Xenon.Infrastructure/Services/ProductService.cs
using Xenon.Domain.Interfaces;
using Xenon.Domain.Models;

public class ProductService : IProductService
{
    private readonly IStoreRepository _repository;

    public ProductService(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string? category = null)
    {
        return await _repository.GetProductsAsync(page, pageSize, category);
    }

    public async Task<Product?> GetProductAsync(long id)
    {
        return await _repository.GetProductAsync(id);
    }

    public async Task<int> GetTotalCountAsync(string? category = null)
    {
        return await _repository.GetTotalCountAsync(category);
    }
}