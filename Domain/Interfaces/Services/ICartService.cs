using Xenon.Domain.Models;

namespace Xenon.Domain.Interfaces.Services
{
    public interface ICartService
    {
        Task AddItemAsync(string cartId, long productId, int quantity = 1);
        Task DeacreaseItemAsync(string cartId, long productId);
        Task RemoveItemAsync(string cartId, long productId);
        Task UpdateQuantityAsync(string cartId, long productId, int quantity);
        Task<Cart> GetCartAsync(string cartId);
        Task ClearCartAsync(string cartId);
        Task<decimal> GetTotalValueAsync(string cartId);

        /// <summary>
        /// Миграция сессионной корзины в корзину пользователя при логине
        /// </summary>
        Task MigrateSessionCartToUserAsync(string sessionId, string userId);
    }
}
