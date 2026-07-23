using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models.ViewModels;

namespace Xenon.Web.Controllers
{
    [Route("{category}/Page{productPage:int}")]
    [Route("Page{productPage:int}")]
    [Route("Home/LoadMore")]
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

        public async Task<IActionResult> Index(string category, int page = 1)
        {
            const int pageSize = 20;

            // Получаем товары с пагинацией
            var products = await _productService.GetProductsAsync(page, pageSize, category);
            var total = await _productService.GetTotalCountAsync(category);

            // Получаем корзину для отображения количества
            var cart = await _cartService.GetCartAsync(CartId);

            // Преобразуем товары с указанием количества в корзине
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
                CurrentCategory = category
            };

            return View(viewModel);
        }

        // Загрузка следующей порции товаров (для бесконечного скролла)
        //public async Task<IActionResult> LoadMore(string category, int page)
        //{
        //    const int pageSize = 20;
        //    var products = await _productService.GetProductsAsync(page, pageSize, category);
        //    var cart = await _cartService.GetCartAsync(CartId);

        //    var productsWithQuantity = products
        //        .Select(p => new ProductCartViewModel
        //        {
        //            Product = p,
        //            QuantityInCart = cart.Lines
        //                .FirstOrDefault(l => l.Product.ProductID == p.ProductID)
        //                ?.Quantity ?? 0
        //        })
        //        .ToList();

        //    return PartialView("_ProductGridItems", productsWithQuantity);
        //}
    }
}