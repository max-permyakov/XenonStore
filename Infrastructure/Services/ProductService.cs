using Xenon.Domain.Models;

public class ProductService : IProductService
{
    private readonly IStoreRepository _repository;

    public ProductService(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, ProductFilter? filter = null)
    {
        return await _repository.GetProductsAsync(page, pageSize, filter);
    }

    public async Task<int> GetTotalCountAsync(ProductFilter? filter = null)
    {
        return await _repository.GetTotalCountAsync(filter);
    }

    public async Task<Product?> GetProductAsync(long id)
    {
        return await _repository.GetProductAsync(id);
    }
}
