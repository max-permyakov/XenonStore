using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Pages.Admin;
using SportsStore.Infrastructure;
namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private  ILogger _logger;
        private IStoreRepository repository;
        public int PageSize = 4;
        private readonly IDistributedCache cache;
        public HomeController(IStoreRepository repo, IDistributedCache distributedCache,ILogger<HomeController> logger)
        {
            _logger = logger;
            repository = repo;
            cache = distributedCache;
        }
        [Route("{category}/Page{productPage:int}")]
        [Route("Page{productPage:int}")]
        [Route("{category}")]
        [Route("")]
        public async Task<ViewResult> Index(string? category, int productPage = 1)
        {
            string cacheKey = $"Products_{category ?? "all"}_{productPage}";
            ProductsListViewModel? model = await cache.GetRecordAsync<ProductsListViewModel>(cacheKey);

            if(model==null)
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
                await cache.SetRecordAsync(cacheKey, model,TimeSpan.FromMinutes(3));
                _logger.LogInformation($"Success Add Page{productPage} for category {category} to cache");
            }
            return View(model);
            
        }
    }
}