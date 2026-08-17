using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Infrastructure.Services
{
    public class EFUserCartStorage : ICartStorage
    {
        private readonly StoreDbContext _context;

        public EFUserCartStorage(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetAsync(string cartId)
        {
            var userCart = await _context.UserCarts
                .Include(uc => uc.Lines)
                    .ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(uc => uc.UserId == cartId);

            if (userCart == null)
                return new Cart();

            return new Cart
            {
                Lines = userCart.Lines.ToList()
            };
        }

        public async Task SaveAsync(string cartId, Cart cart)
        {
            var userCart = await _context.UserCarts
                .Include(uc => uc.Lines)
                .FirstOrDefaultAsync(uc => uc.UserId == cartId);

            if (userCart == null)
            {
                userCart = new UserCart { UserId = cartId };
                _context.UserCarts.Add(userCart);
            }

            userCart.Lines.Clear();
            foreach (var line in cart.Lines)
            {
                userCart.Lines.Add(new CartLine
                {
                    Product = line.Product,
                    Quantity = line.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string cartId)
        {
            var userCart = await _context.UserCarts
                .Include(uc => uc.Lines)
                .FirstOrDefaultAsync(uc => uc.UserId == cartId);

            if (userCart != null)
            {
                _context.UserCarts.Remove(userCart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string cartId)
        {
            return await _context.UserCarts.AnyAsync(uc => uc.UserId == cartId);
        }
    }
}
