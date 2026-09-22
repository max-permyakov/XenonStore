using Xenon.Domain.Models;

namespace Xenon.Domain.Interfaces.Services
{
    public interface IRecentlyViewedService
    {
        Task RecordViewAsync(long productId, string? userId, string? sessionId);
        Task RecordViewsAsync(IEnumerable<long> productIds, string? userId, string? sessionId);
        Task<List<Product>> GetRecentlyViewedAsync(string? userId, string? sessionId, int count = 20);
        Task<int> GetCountAsync(string? userId, string? sessionId);
        Task<int> MergeGuestToUserAsync(string? sessionId, string userId);
    }
}