using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartStorage _sessionStorage;
        private readonly EFUserCartStorage _userCartStorage;
        private readonly IStoreRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartStorage sessionStorage,
            EFUserCartStorage userCartStorage,
            IStoreRepository repository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CartService> logger)
        {
            _sessionStorage = sessionStorage;
            _userCartStorage = userCartStorage;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        private string? UserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        private string SessionId =>
            _httpContextAccessor.HttpContext?.Session?.Id ?? Guid.NewGuid().ToString();

        private ICartStorage CurrentStorage =>
            IsAuthenticated ? _userCartStorage : _sessionStorage;

        private string CurrentCartId =>
            IsAuthenticated ? UserId! : SessionId;

        public async Task AddItemAsync(string cartId, long productId, int quantity = 1)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            var product = await _repository.GetProductAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            var storage = ResolveStorage(cartId);
            var cart = await storage.GetAsync(cartId);
            cart.AddItem(product, quantity);
            await storage.SaveAsync(cartId, cart);

            _logger.LogInformation("Added product {ProductId} x{Quantity} to cart {CartId}",
                productId, quantity, cartId);
        }

        public async Task RemoveItemAsync(string cartId, long productId)
        {
            var storage = ResolveStorage(cartId);
            var cart = await storage.GetAsync(cartId);
            var line = cart.Lines.FirstOrDefault(l => l.Product.ProductID == productId);
            if (line != null)
            {
                cart.RemoveLine(line.Product);
                await storage.SaveAsync(cartId, cart);
                _logger.LogInformation("Removed product {ProductId} from cart {CartId}",
                    productId, cartId);
            }
        }

        public async Task UpdateQuantityAsync(string cartId, long productId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            var storage = ResolveStorage(cartId);
            var cart = await storage.GetAsync(cartId);
            var line = cart.Lines.FirstOrDefault(l => l.Product.ProductID == productId);

            if (line == null)
                throw new KeyNotFoundException($"Product {productId} not in cart");

            if (quantity == 0) cart.RemoveLine(line.Product);
            else line.Quantity = quantity;

            await storage.SaveAsync(cartId, cart);
        }

        public async Task<Cart> GetCartAsync(string cartId)
        {
            var storage = ResolveStorage(cartId);
            return await storage.GetAsync(cartId);
        }

        public async Task ClearCartAsync(string cartId)
        {
            var storage = ResolveStorage(cartId);
            await storage.DeleteAsync(cartId);
            _logger.LogInformation("Cleared cart {CartId}", cartId);
        }

        public async Task<decimal> GetTotalValueAsync(string cartId)
        {
            var storage = ResolveStorage(cartId);
            var cart = await storage.GetAsync(cartId);
            return cart.ComputeTotalValue();
        }

        public async Task DeacreaseItemAsync(string cartId, long productId)
        {
            var storage = ResolveStorage(cartId);
            var cart = await storage.GetAsync(cartId);
            cart.DecreaseItem(productId);
            await storage.SaveAsync(cartId, cart);
        }

        public async Task MigrateSessionCartToUserAsync(string sessionId, string userId)
        {
            var sessionCart = await _sessionStorage.GetAsync(sessionId);
            if (sessionCart.Lines.Count == 0)
                return;

            var userCart = await _userCartStorage.GetAsync(userId);

            foreach (var line in sessionCart.Lines)
            {
                userCart.AddItem(line.Product, line.Quantity);
            }

            await _userCartStorage.SaveAsync(userId, userCart);
            await _sessionStorage.DeleteAsync(sessionId);

            _logger.LogInformation(
                "Migrated {Count} items from session {SessionId} to user {UserId}",
                sessionCart.Lines.Count, sessionId, userId);
        }

        private ICartStorage ResolveStorage(string cartId)
        {
            if (IsAuthenticated && cartId == UserId)
                return _userCartStorage;
            if (!IsAuthenticated && cartId == SessionId)
                return _sessionStorage;

            if (Guid.TryParse(cartId, out _))
                return _sessionStorage;

            return _userCartStorage;
        }
    }
}
