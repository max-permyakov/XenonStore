using Microsoft.EntityFrameworkCore;
using SportsStore.Infrastructure;

namespace SportsStore.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            StoreDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Если таблица Products пуста, генерируем 200 товаров
            if (!context.Products.Any())
            {
                var products = DataGenerator.GenerateProducts(2000);
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            // Если заказов нет, генерируем 30 заказов с позициями на основе существующих товаров
            if (!context.Orders.Any())
            {
                var products = context.Products.ToList();
                var orders = DataGenerator.GenerateOrders(products, 30);
                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
        }
    }
}