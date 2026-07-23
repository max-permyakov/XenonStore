using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Models;
using Xenon.Web.Models.ViewModels;


namespace Xenon.Web.Controllers
{
    public class HomeController : Controller
    {
        private  ILogger _logger;
        private IStoreRepository repository;
        public int PageSize = 20;
        
        private readonly Cart cart;
        public HomeController(IStoreRepository repo,ILogger<HomeController> logger, Cart cartService)
        {
            _logger = logger;
            repository = repo;
            
            cart = cartService;

        }
        [Route("{category}/Page{productPage:int}")]
        [Route("Page{productPage:int}")]
        [Route("Home/LoadMore")]
        [Route("{category}")]
        [Route("")]
        public async Task<ViewResult> Index(string? category, int productPage = 1)
        {
         
            var products = repository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize)
                .ToList(); 

          
            int totalItems = category == null
                ? repository.Products.Count()
                : repository.Products.Count(e => e.Category == category);

            var productsWithQuantity = products.Select(p => new ProductCartViewModel
            {
                Product = p,
                QuantityInCart = cart.Lines
                    .Where(l => l.Product.ProductID == p.ProductID)
                    .Select(l => l.Quantity)
                    .FirstOrDefault()
            });

            var model = new ProductsListWithCartViewModel
            {
                Products = productsWithQuantity,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = totalItems
                },
                CurrentCategory = category
            };

            return View(model);
        }

        
    }
}