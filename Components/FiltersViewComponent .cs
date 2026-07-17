using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Infrastructure;
namespace SportsStore.Components
{
    public class FiltersViewComponent : ViewComponent
    {
        private IStoreRepository repository;
        private  IDistributedCache _cache;
        private readonly ILogger _logger;
        public FiltersViewComponent(IStoreRepository repo,IDistributedCache cache,ILogger<NavigationMenuViewComponent> logger)
        {
            repository = repo;
            _cache = cache;
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string cacheKey = $"Categoties";
            List<string>? categories = await _cache.GetRecordAsync<List<string>>(cacheKey);
            if (categories == null)
            {
                categories = repository.Products
                    .Select(x => x.Category)
                    .Distinct()
                    .OrderBy(x => x).ToList();
                await _cache.SetRecordAsync(cacheKey, categories,TimeSpan.FromSeconds(60));
                _logger.LogInformation("Success add categories to cache");
            }
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(categories);
        }
    }
}