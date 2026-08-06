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

            var filter = BuildFilter();
            filter.Category = category;
            filter.SearchTerm = searchTerm;

            var products = await _productService.GetProductsAsync(page, pageSize, filter);
            var total = await _productService.GetTotalCountAsync(filter);
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

            var filter = BuildFilter();
            filter.Category = category;
            filter.SearchTerm = searchTerm;

            var products = await _productService.GetProductsAsync(page, pageSize, filter);
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

        private ProductFilter BuildFilter()
        {
            var request = Request.Query;

            var filter = new ProductFilter
            {
                MinPrice = ParseDecimal(request["minPrice"]),
                MaxPrice = ParseDecimal(request["maxPrice"]),
                MinRating = ParseDouble(request["minRating"]),
                Supplier = !string.IsNullOrWhiteSpace(request["supplier"]) ? request["supplier"].ToString() : null
            };

            var sort = request["sort"].ToString();
            var direction = string.Equals(request["dir"].ToString(), "asc", StringComparison.OrdinalIgnoreCase)
                ? SortDirection.Ascending
                : SortDirection.Descending;

            switch (sort)
            {
                case "price-asc":
                    filter.SortBy = ProductSortBy.Price;
                    filter.Direction = SortDirection.Ascending;
                    break;
                case "price-desc":
                    filter.SortBy = ProductSortBy.Price;
                    filter.Direction = SortDirection.Descending;
                    break;
                case "rating-asc":
                    filter.SortBy = ProductSortBy.Rating;
                    filter.Direction = SortDirection.Ascending;
                    break;
                case "rating-desc":
                    filter.SortBy = ProductSortBy.Rating;
                    filter.Direction = SortDirection.Descending;
                    break;
                case "popularity-asc":
                    filter.SortBy = ProductSortBy.Popularity;
                    filter.Direction = SortDirection.Ascending;
                    break;
                default:
                    filter.SortBy = ProductSortBy.Popularity;
                    filter.Direction = SortDirection.Descending;
                    break;
            }

            return filter;
        }

        private static decimal? ParseDecimal(string? value)
        {
            return decimal.TryParse(value, out var parsed) ? parsed : null;
        }

        private static double? ParseDouble(string? value)
        {
            return double.TryParse(value, out var parsed) ? parsed : null;
        }
    }
}
