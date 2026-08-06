using Microsoft.Extensions.Logging;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartStorage _cartStorage;
        private readonly IStoreRepository _repository;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartStorage cartStorage,
            IStoreRepository repository,
            ILogger<CartService> logger)
        {
            _cartStorage = cartStorage;
            _repository = repository;
            _logger = logger;
        }

        public async Task AddItemAsync(string cartId, long productId, int quantity = 1)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            var product = await _repository.GetProductAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            // Здесь можно добавить бизнес-правила:
            // - Проверка наличия на складе
            // - Максимальное количество товара в одной позиции
            // - Логирование добавления

            var cart = await _cartStorage.GetAsync(cartId);
            cart.AddItem(product, quantity);
            await _cartStorage.SaveAsync(cartId, cart);

            _logger.LogInformation("Added product {ProductId} x{Quantity} to cart {CartId}",
                productId, quantity, cartId);
        }

        public async Task RemoveItemAsync(string cartId, long productId)
        {
            var cart = await _cartStorage.GetAsync(cartId);
            var line = cart.Lines.FirstOrDefault(l => l.Product.ProductID == productId);
            if (line != null)
            {
                cart.RemoveLine(line.Product);
                await _cartStorage.SaveAsync(cartId, cart);
                _logger.LogInformation("Removed product {ProductId} from cart {CartId}",
                    productId, cartId);
            }
        }

        public async Task UpdateQuantityAsync(string cartId, long productId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            var cart = await _cartStorage.GetAsync(cartId);
            var line = cart.Lines.FirstOrDefault(l => l.Product.ProductID == productId);

            if (line == null)
                throw new KeyNotFoundException($"Product {productId} not in cart");

            if (quantity == 0)
            {
                cart.RemoveLine(line.Product);
            }
            else
            {
                line.Quantity = quantity;
            }

            await _cartStorage.SaveAsync(cartId, cart);
            _logger.LogDebug("Updated product {ProductId} quantity to {Quantity}",
                productId, quantity);
        }

        public async Task<Cart> GetCartAsync(string cartId)
        {
            return await _cartStorage.GetAsync(cartId);
        }

        public async Task ClearCartAsync(string cartId)
        {
            await _cartStorage.DeleteAsync(cartId);
            _logger.LogInformation("Cleared cart {CartId}", cartId);
        }

        public async Task<decimal> GetTotalValueAsync(string cartId)
        {
            var cart = await _cartStorage.GetAsync(cartId);
            return cart.ComputeTotalValue();
        }

        public async Task DeacreaseItemAsync(string cartId, long productId)
        {
            var cart = await _cartStorage.GetAsync(cartId);
            cart.DecreaseItem(productId);
            await _cartStorage.SaveAsync(cartId, cart);
            _logger.LogDebug("Decreased product {ProductId} in cart {CartId}",
                productId, cartId);
        }
    }
}