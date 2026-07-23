using Microsoft.EntityFrameworkCore;
using Xenon.Infrastructure.Data;
using Xenon.Domain.Models;
using Xenon.Domain.Interfaces;

namespace Xenon.Infrastructure.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private StoreDbContext context;
        public EFOrderRepository(StoreDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Order> Orders => context.Orders
                            .Include(o => o.Lines)
.ThenInclude(l => l.Product);
        public void SaveOrder(Order order)
        {
            context.AttachRange(order.Lines.Select(l => l.Product));
            if (order.OrderID == 0)
            {
                context.Orders.Add(order);
            }
            
            context.SaveChanges();
        }
    }
}