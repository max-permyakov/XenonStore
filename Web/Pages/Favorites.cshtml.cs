using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models;
using Xenon.Web.Models.ViewModels;

namespace Xenon.Web.Pages
{
    public class FavoritesModel : PageModel
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FavoritesModel(
            IFavoriteService favoriteService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor)
        {
            _favoriteService = favoriteService;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<ProductCartViewModel> Items { get; set; } = new List<ProductCartViewModel>();
        public string ReturnUrl { get; set; } = "/";

        private (string? UserId, string? SessionId) Owner =>
            FavoriteOwner.Resolve(HttpContext.User, HttpContext);

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

        public async Task OnGet(string returnUrl)
        {
            ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;

            var products = await _favoriteService.GetFavoriteProductsAsync(Owner.UserId, Owner.SessionId);
            var cart = await _cartService.GetCartAsync(GetCartId());

            Items = products
                .Select(p => new ProductCartViewModel
                {
                    Product = p,
                    QuantityInCart = cart.Lines
                        .FirstOrDefault(l => l.Product.ProductID == p.ProductID)
                        ?.Quantity ?? 0,
                    IsFavorite = true
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostToggle(long productId, string returnUrl)
        {
            var result = await _favoriteService.ToggleAsync(productId, Owner.UserId, Owner.SessionId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return new JsonResult(new { isFavorite = result.IsFavorite, count = result.Count });
            }

            return RedirectToPage(new { returnUrl });
        }

        public async Task<IActionResult> OnPostRemove(long productId, string returnUrl)
        {
            await _favoriteService.ToggleAsync(productId, Owner.UserId, Owner.SessionId);
            return RedirectToPage(new { returnUrl });
        }
    }
}
