using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Infrastructure.Services
{
    public class RecentlyViewedService : IRecentlyViewedService
    {
        private const int MaxHistoryPerOwner = 100;
        private readonly StoreDbContext _context;
        private readonly ILogger<RecentlyViewedService> _logger;

        public RecentlyViewedService(StoreDbContext context, ILogger<RecentlyViewedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RecordViewAsync(long productId, string? userId, string? sessionId)
        {
            await RecordViewsAsync(new[] { productId }, userId, sessionId);
        }

        public async Task RecordViewsAsync(IEnumerable<long> productIds, string? userId, string? sessionId)
        {
            var ids = productIds.Distinct().ToList();
            if (ids.Count == 0)
                return;

            var now = DateTime.UtcNow;
            var owner = QueryForOwner(userId, sessionId);

            foreach (var productId in ids)
            {
                var existing = await owner.FirstOrDefaultAsync(r => r.ProductId == productId);
                if (existing != null)
                {
                    existing.ViewedAt = now;
                }
                else
                {
                    _context.RecentlyViewedProducts.Add(new RecentlyViewed
                    {
                        ProductId = productId,
                        UserId = userId,
                        SessionId = sessionId,
                        ViewedAt = now
                    });
                }
            }

            await _context.SaveChangesAsync();
            await TrimHistoryAsync(userId, sessionId);
        }

        public async Task<List<Product>> GetRecentlyViewedAsync(string? userId, string? sessionId, int count = 20)
        {
            var ids = await QueryForOwner(userId, sessionId)
                .OrderByDescending(r => r.ViewedAt)
                .Take(count)
                .Select(r => r.ProductId)
                .ToListAsync();

            if (ids.Count == 0)
                return new List<Product>();

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => ids.Contains(p.ProductID))
                .ToListAsync();

            var byId = products.ToDictionary(p => p.ProductID);
            return ids
                .Where(id => byId.ContainsKey(id))
                .Select(id => byId[id])
                .ToList();
        }

        public async Task<int> GetCountAsync(string? userId, string? sessionId)
        {
            return await QueryForOwner(userId, sessionId).CountAsync();
        }

        public async Task<int> MergeGuestToUserAsync(string? sessionId, string userId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return 0;

            var guestRows = await _context.RecentlyViewedProducts
                .Where(r => r.SessionId == sessionId)
                .ToListAsync();

            if (guestRows.Count == 0)
                return 0;

            var existingUserRows = await _context.RecentlyViewedProducts
                .Where(r => r.UserId == userId)
                .ToDictionaryAsync(r => r.ProductId);

            var merged = 0;
            foreach (var guest in guestRows)
            {
                if (existingUserRows.TryGetValue(guest.ProductId, out var userRow))
                {
                    if (guest.ViewedAt > userRow.ViewedAt)
                        userRow.ViewedAt = guest.ViewedAt;
                    _context.RecentlyViewedProducts.Remove(guest);
                }
                else
                {
                    guest.SessionId = null;
                    guest.UserId = userId;
                    merged++;
                }
            }

            await _context.SaveChangesAsync();
            await TrimHistoryAsync(userId, null);
            return merged;
        }

        private IQueryable<RecentlyViewed> QueryForOwner(string? userId, string? sessionId)
        {
            if (!string.IsNullOrEmpty(userId))
                return _context.RecentlyViewedProducts.Where(r => r.UserId == userId);
            return _context.RecentlyViewedProducts.Where(r => r.SessionId == sessionId);
        }

        private async Task TrimHistoryAsync(string? userId, string? sessionId)
        {
            var owner = QueryForOwner(userId, sessionId);
            var keepIds = await owner
                .OrderByDescending(r => r.ViewedAt)
                .Take(MaxHistoryPerOwner)
                .Select(r => r.RecentlyViewedId)
                .ToListAsync();

            var toRemove = await owner
                .Where(r => !keepIds.Contains(r.RecentlyViewedId))
                .ToListAsync();

            if (toRemove.Count > 0)
            {
                _context.RecentlyViewedProducts.RemoveRange(toRemove);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Trimmed {Count} stale recently-viewed rows", toRemove.Count);
            }
        }
    }
}