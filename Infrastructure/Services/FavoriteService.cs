using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Infrastructure.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly StoreDbContext _context;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(StoreDbContext context, ILogger<FavoriteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private IQueryable<Favorite> ForOwner(string? userId, string? sessionId)
        {
            if (!string.IsNullOrEmpty(userId))
                return _context.Favorites.Where(f => f.UserId == userId);
            return _context.Favorites.Where(f => f.SessionId == sessionId);
        }

        public async Task<List<long>> GetFavoriteProductIdsAsync(string? userId, string? sessionId)
        {
            return await ForOwner(userId, sessionId)
                .Select(f => f.ProductId)
                .ToListAsync();
        }

        public async Task<List<Product>> GetFavoriteProductsAsync(string? userId, string? sessionId)
        {
            var ids = await ForOwner(userId, sessionId)
                .Select(f => f.ProductId)
                .ToListAsync();

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => ids.Contains(p.ProductID))
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string? userId, string? sessionId)
        {
            return await ForOwner(userId, sessionId).CountAsync();
        }

        public async Task<bool> IsFavoriteAsync(long productId, string? userId, string? sessionId)
        {
            return await ForOwner(userId, sessionId)
                .AnyAsync(f => f.ProductId == productId);
        }

        public async Task<(bool IsFavorite, int Count)> ToggleAsync(long productId, string? userId, string? sessionId)
        {
            var owner = ForOwner(userId, sessionId);
            var existing = await owner.FirstOrDefaultAsync(f => f.ProductId == productId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                _logger.LogInformation("Removed product {ProductId} from favorites", productId);
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    ProductId = productId,
                    UserId = userId,
                    SessionId = sessionId
                });
                _logger.LogInformation("Added product {ProductId} to favorites", productId);
            }

            await _context.SaveChangesAsync();
            var count = await owner.CountAsync();
            return (existing == null, count);
        }
    }
}
