using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Pages.Admin;
namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private  ILogger _logger;
        private IStoreRepository repository;
        public int PageSize = 4;
        private readonly IMemoryCache cache;
        public HomeController(IStoreRepository repo, IMemoryCache memoryCache,ILogger<HomeController> logger)
        {
            _logger = logger;
            repository = repo;
            cache = memoryCache;
        }
        public ViewResult Index(string? category, int productPage = 1)
        {
            string cacheKey = $"Products_{category ?? "all"}_{productPage}";
            if(!cache.TryGetValue(cacheKey,out ProductsListViewModel model))
            {
                var products = repository.Products
                    .Where(p => category == null || p.Category == category)
                   .OrderBy(p => p.ProductID)
                   .Skip((productPage - 1) * PageSize)
                   .Take(PageSize).ToList();
                int totalItems = category == null
                ? repository.Products.Count()
                : repository.Products.Count(e => e.Category == category);
                model = new ProductsListViewModel
                {
                    Products = products,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = productPage,
                        ItemsPerPage = PageSize,
                        TotalItems = totalItems
                    },
                    CurrentCategory = category
                };
                cache.Set(cacheKey, model,TimeSpan.FromMinutes(5));
                _logger.LogInformation($"Success Add Page{productPage} for category {category} in cache");
            }
            return View(model);
            
        }
    }
}