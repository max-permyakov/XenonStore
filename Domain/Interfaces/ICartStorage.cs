using System;
using System.Collections.Generic;
using System.Text;
using Xenon.Domain.Models;

// Xenon.Domain/Interfaces/Services/ICartStorage.cs
namespace Xenon.Domain.Interfaces.Services
{
    public interface ICartStorage
    {
        /// <summary>
        /// Получить корзину по идентификатору
        /// </summary>
        Task<Cart> GetAsync(string cartId);

        /// <summary>
        /// Сохранить корзину
        /// </summary>
        Task SaveAsync(string cartId, Cart cart);

        /// <summary>
        /// Удалить корзину
        /// </summary>
        Task DeleteAsync(string cartId);

        /// <summary>
        /// Проверить, существует ли корзина
        /// </summary>
        Task<bool> ExistsAsync(string cartId);
    }
}