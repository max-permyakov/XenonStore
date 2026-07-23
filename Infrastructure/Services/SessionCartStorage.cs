using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Services
{
    public class SessionCartStorage : ICartStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SessionCartStorage> _logger;
        private const string CartSessionKey = "Cart";

        public SessionCartStorage(
            IHttpContextAccessor httpContextAccessor,
            ILogger<SessionCartStorage> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("Session not available");

        public async Task<Cart> GetAsync(string cartId)
        {
            try
            {
                var data = Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(data))
                    return new Cart();

                var cart = JsonSerializer.Deserialize<Cart>(data);
                return cart ?? new Cart();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize cart from session");
                return new Cart();
            }
        }

        public async Task SaveAsync(string cartId, Cart cart)
        {
            var data = JsonSerializer.Serialize(cart);
            Session.SetString(CartSessionKey, data);
            _logger.LogDebug("Cart saved to session");
        }

        public async Task DeleteAsync(string cartId)
        {
            Session.Remove(CartSessionKey);
            _logger.LogDebug("Cart removed from session");
        }

        public async Task<bool> ExistsAsync(string cartId)
        {
            return Session.Keys.Contains(CartSessionKey);
        }
    }
}