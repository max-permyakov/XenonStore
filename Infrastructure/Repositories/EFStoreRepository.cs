using Xenon.Infrastructure.Data;
using Xenon.Domain.Models;
using Xenon.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Xenon.Infrastructure.Repositories
{
    public class EFStoreRepository : IStoreRepository
    {
        private StoreDbContext context;
        public EFStoreRepository(StoreDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Product> Products => context.Products;
        public void CreateProduct(Product p)
        {
            context.Add(p);
            context.SaveChanges();
        }
        public void DeleteProduct(Product p)
        {
            context.Remove(p);
            context.SaveChanges();
        }

        public Task<Product?> GetProductAsync(long id)
        {
            return context.Products.FirstOrDefaultAsync(x=>x.ProductID == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string? category = null)
        {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
               
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            return await query
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, string? category = null, string? searchTerm = null)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var normalizedSearch = searchTerm.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(normalizedSearch) ||
                    (p.Description != null && p.Description.ToLower().Contains(normalizedSearch)) ||
                    (p.Features != null && p.Features.ToLower().Contains(normalizedSearch))
                );
            }

            return await query
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? category = null)
        {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetTotalCountAsync(string? category = null, string? searchTerm = null)
        {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var normalizedSearch = searchTerm.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(normalizedSearch) ||
                    (p.Description != null && p.Description.ToLower().Contains(normalizedSearch)) ||
                    (p.Features != null && p.Features.ToLower().Contains(normalizedSearch))
                );
            }

            return await query.CountAsync();
        }

        public void SaveProduct(Product p)
        {
            context.SaveChanges();
        }
    }
}