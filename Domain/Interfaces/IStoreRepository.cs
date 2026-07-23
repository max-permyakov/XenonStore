using Xenon.Domain.Models;
namespace Xenon.Domain.Interfaces
{
    public interface IStoreRepository
    {
        IQueryable<Product> Products { get; }
        void SaveProduct(Product p);
        void CreateProduct(Product p);
        void DeleteProduct(Product p);
        Task<Product> GetProductAsync(long id);
        Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string category);
        Task<int> GetTotalCountAsync(string category);
    }
}