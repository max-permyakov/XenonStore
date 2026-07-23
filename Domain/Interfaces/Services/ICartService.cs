using Xenon.Domain.Models;

// Xenon.Domain/Interfaces/Services/ICartService.cs
namespace Xenon.Domain.Interfaces.Services
{
    public interface ICartService
    {
        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        Task AddItemAsync(string cartId, long productId, int quantity = 1);
        Task DeacreaseItemAsync(string cartId, long productId);

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        Task RemoveItemAsync(string cartId, long productId);

        /// <summary>
        /// Обновить количество товара
        /// </summary>
        Task UpdateQuantityAsync(string cartId, long productId, int quantity);

        /// <summary>
        /// Получить корзину
        /// </summary>
        Task<Cart> GetCartAsync(string cartId);

        /// <summary>
        /// Очистить корзину
        /// </summary>
        Task ClearCartAsync(string cartId);

        /// <summary>
        /// Получить общую стоимость
        /// </summary>
        Task<decimal> GetTotalValueAsync(string cartId);

        
    }
}