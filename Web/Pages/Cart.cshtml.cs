using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Web.Pages
{
    public class CartModel : PageModel
    {
        private readonly ILogger<CartModel> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartModel(
            ILogger<CartModel> logger,
            IProductService productService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Свойства для представления
        public Cart Cart { get; set; } = new Cart();
        public string ReturnUrl { get; set; } = "/";

        // ID корзины из сессии
        private string CartId => _httpContextAccessor.HttpContext?.Session?.Id
            ?? Guid.NewGuid().ToString();

        // GET: отображение корзины
        public async Task OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            Cart = await _cartService.GetCartAsync(CartId);
            _logger.LogDebug("Cart loaded with {Count} items", Cart.Lines.Count());
        }

        // Добавление товара (обычный POST)
        public async Task<IActionResult> OnPost(long productId, string returnUrl)
        {
            try
            {
                await _cartService.AddItemAsync(CartId, productId, 1);
                _logger.LogInformation("Product {ProductId} added to cart", productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product {ProductId} to cart", productId);
                // Можно добавить сообщение об ошибке в TempData
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }

        // Добавление одного товара (из списка товаров)
        public async Task<IActionResult> OnPostAddOneAsync(long productId, string returnUrl)
        {
            await _cartService.AddItemAsync(CartId, productId, 1);
            var cart = await _cartService.GetCartAsync(CartId);
            int quantity = cart.Lines
                .FirstOrDefault(l => l.Product.ProductID == productId)?.Quantity ?? 0;

            // Если запрос AJAX, возвращаем JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return new JsonResult(new { quantity });
            }

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> OnPostRemoveOneAsync(long productId, string returnUrl)
        {
            await _cartService.DeacreaseItemAsync(CartId, productId);
            var cart = await _cartService.GetCartAsync(CartId);
            int quantity = cart.Lines
                .FirstOrDefault(l => l.Product.ProductID == productId)?.Quantity ?? 0;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return new JsonResult(new { quantity });
            }

            return LocalRedirect(returnUrl);
        }


        public async Task<IActionResult> OnPostRemove(long productId, string returnUrl)
        {
            try
            {
                await _cartService.RemoveItemAsync(CartId, productId);
                _logger.LogInformation("Product {ProductId} removed from cart", productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove product {ProductId}", productId);
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }

        // Очистка корзины
        public async Task<IActionResult> OnPostClear()
        {
            await _cartService.ClearCartAsync(CartId);
            _logger.LogInformation("Cart cleared");
            return RedirectToPage();
        }
    }
}