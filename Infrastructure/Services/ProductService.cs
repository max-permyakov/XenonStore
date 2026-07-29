using Xenon.Domain.Models;

public class ProductService : IProductService
{
    private readonly IStoreRepository _repository;

    public ProductService(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string? category = null, string? searchTerm = null)
    {
        return await _repository.GetProductsAsync(page, pageSize, category, searchTerm);
    }

    public async Task<int> GetTotalCountAsync(string? category = null, string? searchTerm = null)
    {
        return await _repository.GetTotalCountAsync(category, searchTerm);
    }

    public async Task<Product?> GetProductAsync(long id)
    {
        return await _repository.GetProductAsync(id);
    }
}