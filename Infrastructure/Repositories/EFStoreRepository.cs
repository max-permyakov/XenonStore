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
            return context.Products.FirstOrDefaultAsync(x => x.ProductID == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize, ProductFilter? filter = null)
        {
            var query = BuildFilteredQuery(filter);

            query = ApplySorting(query, filter);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ProductFilter? filter = null)
        {
            var query = BuildFilteredQuery(filter);
            return await query.CountAsync();
        }

        private IQueryable<Product> BuildFilteredQuery(ProductFilter? filter)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (filter == null)
            {
                return query;
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == filter.Category);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var normalizedSearch = filter.SearchTerm.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(normalizedSearch) ||
                    (p.Description != null && p.Description.ToLower().Contains(normalizedSearch)) ||
                    (p.Features != null && p.Features.ToLower().Contains(normalizedSearch))
                );
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(p => p.Rating != null && p.Rating >= filter.MinRating.Value);
            }

            if (!string.IsNullOrEmpty(filter.Supplier))
            {
                query = query.Where(p => p.Supplier != null && p.Supplier.Name == filter.Supplier);
            }

            return query;
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, ProductFilter? filter)
        {
            var direction = filter?.Direction ?? SortDirection.Descending;
            var sortBy = filter?.SortBy ?? ProductSortBy.Popularity;

            return sortBy switch
            {
                ProductSortBy.Price => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Price).ThenBy(p => p.ProductID)
                    : query.OrderByDescending(p => p.Price).ThenBy(p => p.ProductID),
                ProductSortBy.Rating => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Rating ?? 0).ThenBy(p => p.ProductID)
                    : query.OrderByDescending(p => p.Rating ?? 0).ThenBy(p => p.ProductID),
                _ => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Popularity).ThenBy(p => p.ProductID)
                    : query.OrderByDescending(p => p.Popularity).ThenBy(p => p.ProductID)
            };
        }

        public void SaveProduct(Product p)
        {
            context.SaveChanges();
        }
    }
}
