using Xenon.Domain.Models;

namespace Xenon.Domain.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task<List<long>> GetFavoriteProductIdsAsync(string? userId, string? sessionId);
        Task<List<Product>> GetFavoriteProductsAsync(string? userId, string? sessionId);
        Task<int> GetCountAsync(string? userId, string? sessionId);
        Task<bool> IsFavoriteAsync(long productId, string? userId, string? sessionId);
        Task<(bool IsFavorite, int Count)> ToggleAsync(long productId, string? userId, string? sessionId);
    }
}
