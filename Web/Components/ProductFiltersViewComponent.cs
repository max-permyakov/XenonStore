using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Interfaces;

namespace Xenon.Web.Components
{
    public class ProductFiltersViewComponent : ViewComponent
    {
        private readonly IStoreRepository _repository;

        public ProductFiltersViewComponent(IStoreRepository repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var query = _repository.Products;

            var suppliers = await query
                .Where(p => p.Supplier != null)
                .Select(p => p.Supplier!.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            var categories = await query
                .Where(p => p.Category != null)
                .Select(p => p.Category!.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            var minPrice = await query.MinAsync(p => (decimal?)p.Price) ?? 0;
            var maxPrice = await query.MaxAsync(p => (decimal?)p.Price) ?? 0;

            ViewBag.Suppliers = suppliers;
            ViewBag.Categories = categories;
            ViewBag.PriceMin = minPrice;
            ViewBag.PriceMax = maxPrice;

            return View();
        }
    }
}
