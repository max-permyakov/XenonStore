using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models;

namespace Xenon.Web.Pages
{
    public class CartModel : PageModel
    {
        private readonly ILogger<CartModel> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IRecentlyViewedService _recentlyViewedService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartModel(
            ILogger<CartModel> logger,
            IProductService productService,
            ICartService cartService,
            IRecentlyViewedService recentlyViewedService,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _recentlyViewedService = recentlyViewedService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Cart Cart { get; set; } = new Cart();
        public string ReturnUrl { get; set; } = "/";

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

        private async Task RecordViewAsync(long productId, (string? UserId, string? SessionId) owner)
        {
            try
            {
                await _recentlyViewedService.RecordViewAsync(productId, owner.UserId, owner.SessionId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to record recently viewed product {ProductId}", productId);
            }
        }

        public async Task OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            Cart = await _cartService.GetCartAsync(GetCartId());
        }

        public async Task<IActionResult> OnPost(long productId, string returnUrl)
        {
            try
            {
                await _cartService.AddItemAsync(GetCartId(), productId, 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product {ProductId} to cart", productId);
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }

        public async Task<IActionResult> OnPostAddOneAsync(long productId, string returnUrl)
        {
            await _cartService.AddItemAsync(GetCartId(), productId, 1);
            var cart = await _cartService.GetCartAsync(GetCartId());
            int quantity = cart.Lines
                .FirstOrDefault(l => l.Product.ProductID == productId)?.Quantity ?? 0;

            var owner = FavoriteOwner.Resolve(User, _httpContextAccessor.HttpContext);
            await RecordViewAsync(productId, owner);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return new JsonResult(new { quantity });

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> OnPostRemoveOneAsync(long productId, string returnUrl)
        {
            await _cartService.DeacreaseItemAsync(GetCartId(), productId);
            var cart = await _cartService.GetCartAsync(GetCartId());
            int quantity = cart.Lines
                .FirstOrDefault(l => l.Product.ProductID == productId)?.Quantity ?? 0;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return new JsonResult(new { quantity });

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> OnPostRemove(long productId, string returnUrl)
        {
            try
            {
                await _cartService.RemoveItemAsync(GetCartId(), productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove product {ProductId} from cart", productId);
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }

        public async Task<IActionResult> OnPostClear()
        {
            await _cartService.ClearCartAsync(GetCartId());
            return RedirectToPage();
        }
    }
}
