using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Infrastructure.Services
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
            if (!context.Products.Any())
            {
                context.SaveChanges();
            }

            if (context.Products.Any() && !context.Products.Any(p => p.Popularity > 0))
            {
                var random = new Random(42);
                foreach (var p in context.Products.OrderBy(p => p.ProductID))
                {
                    p.Popularity = random.Next(1, 500);
                }
                context.SaveChanges();
            }
        }
    }
}