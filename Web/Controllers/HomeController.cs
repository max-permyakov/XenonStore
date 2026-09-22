using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Entities;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models;
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
        private readonly IFavoriteService _favoriteService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IProductService productService,
            ICartService cartService,
            IFavoriteService favoriteService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HomeController> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _favoriteService = favoriteService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private string GetCartId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                    return userId;
            }
            return _httpContextAccessor.HttpContext?.Session?.Id ?? Guid.NewGuid().ToString();
        }

        public async Task<IActionResult> Index(string category, string searchTerm, int page = 1)
        {
            const int pageSize = 20;

            var filter = BuildFilter();
            filter.Category = category;
            filter.SearchTerm = searchTerm;

            var products = await _productService.GetProductsAsync(page, pageSize, filter);
            var total = await _productService.GetTotalCountAsync(filter);

            var owner = FavoriteOwner.Resolve(User, _httpContextAccessor.HttpContext);
            var productsWithQuantity = await BuildProductViewModelsAsync(products, owner);

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

            var owner = FavoriteOwner.Resolve(User, _httpContextAccessor.HttpContext);
            var productsWithQuantity = await BuildProductViewModelsAsync(products, owner);

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

            var owner = FavoriteOwner.Resolve(User, _httpContextAccessor.HttpContext);
            var items = await BuildProductViewModelsAsync(new[] { product }, owner);
            var item = items[0];

            ViewData["CartReturnUrl"] = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            return PartialView("_ProductGridItems", items);
        }

        private async Task<List<ProductCartViewModel>> BuildProductViewModelsAsync(
            IEnumerable<Product> products, (string? UserId, string? SessionId) owner)
        {
            var cart = await _cartService.GetCartAsync(GetCartId());
            var favoriteIds = await _favoriteService.GetFavoriteProductIdsAsync(owner.UserId, owner.SessionId);
            var favoriteSet = favoriteIds.ToHashSet();

            return products
                .Select(p => new ProductCartViewModel
                {
                    Product = p,
                    QuantityInCart = cart.Lines
                        .FirstOrDefault(l => l.Product.ProductID == p.ProductID)
                        ?.Quantity ?? 0,
                    IsFavorite = favoriteSet.Contains(p.ProductID)
                })
                .ToList();
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
