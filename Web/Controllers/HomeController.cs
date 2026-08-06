using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models.ViewModels;

namespace Xenon.Web.Controllers
{
    [Route("{category}/Page{productPage:int}")]
    [Route("Page{productPage:int}")]
    
    [Route("{category}")]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IProductService productService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HomeController> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private string CartId => _httpContextAccessor.HttpContext?.Session?.Id
            ?? Guid.NewGuid().ToString();

        public async Task<IActionResult> Index(string category, string searchTerm, int page = 1)
        {
            const int pageSize = 20;

            var products = await _productService.GetProductsAsync(page, pageSize, category, searchTerm);
            var total = await _productService.GetTotalCountAsync(category, searchTerm);
            var cart = await _cartService.GetCartAsync(CartId);

            var productsWithQuantity = products
                .Select(p => new ProductCartViewModel
                {
                    Product = p,
                    QuantityInCart = cart.Lines
                        .FirstOrDefault(l => l.Product.ProductID == p.ProductID)
                        ?.Quantity ?? 0
                })
                .ToList();

            var viewModel = new ProductsListWithCartViewModel
            {
                Products = productsWithQuantity,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = total
                },
                CurrentCategory = category,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        [Route("Home/LoadMore")]
        public async Task<IActionResult> LoadMore(string category, string searchTerm, int page)
        {
            const int pageSize = 20;
            var products = await _productService.GetProductsAsync(page, pageSize, category, searchTerm);
            var cart = await _cartService.GetCartAsync(CartId);

            var productsWithQuantity = products
                .Select(p => new ProductCartViewModel
                {
                    Product = p,
                    QuantityInCart = cart.Lines
                        .FirstOrDefault(l => l.Product.ProductID == p.ProductID)
                        ?.Quantity ?? 0
                })
                .ToList();

            return PartialView("_ProductGridItems", productsWithQuantity);
        }

        [Route("Home/ProductCard")]
        public async Task<IActionResult> ProductCard(long productId, string returnUrl)
        {
            var product = await _productService.GetProductAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await _cartService.GetCartAsync(CartId);
            var item = new ProductCartViewModel
            {
                Product = product,
                QuantityInCart = cart.Lines
                    .FirstOrDefault(l => l.Product.ProductID == product.ProductID)
                    ?.Quantity ?? 0
            };

            ViewData["CartReturnUrl"] = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            return PartialView("_ProductGridItems", new[] { item });
        }

       
    }
}